using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server_for_ChatApp
{
    enum MessageId : byte
    {
        LOG_IN = 1,
        GET_USERS = 2,
        SEND_MESSAGE = 3,
        LOG_OUT = 4,
        EXISTING_USER_LOG_IN = 5,
        FETCH_OFFLINE_MESSAGES = 6, 
    }


    class TCPServer
    {
        private TcpListener listener;
        private bool isRunning;

        // User ID Dictionary
        public Random_User_ID _idManager;

        UserDictionary User_Logs = new UserDictionary();

        // Active Connections Dictionary
        private Dictionary<string, NetworkStream> Active_Connections = new Dictionary<string, NetworkStream>();

        public TCPServer(int port, Random_User_ID idManager)
        {
            listener = new TcpListener(IPAddress.Loopback, port);
            _idManager = idManager;
        }

        public void Start()
        {
            listener.Start();
            isRunning = true;
            Console.WriteLine("Server started...");

            while (isRunning)
            {
                TcpClient client = listener.AcceptTcpClient();
                System.Threading.ThreadPool.QueueUserWorkItem(state => HandleClient(client));
            }
        }

        public void BroadcastUserList()
        {
            byte[] fullListPacket = Send_User_List.GenerateUserListPacket(_idManager);

            foreach (var activeStream in Active_Connections.Values.ToList())
            {
                try
                {
                    send_packet(activeStream, fullListPacket);
                }
                catch (Exception)
                {
                }
            }
        }
        public void send_packet(NetworkStream stream, byte[] packet)
        {
            //BinaryReader reader = new BinaryReader(new MemoryStream(packet));
            stream.Write(packet, 0, packet.Length);
            stream.Flush();
        }



        public void HandleClient(TcpClient client)
        {
            NetworkStream stream = client.GetStream();
            string currentUsername = "";

            byte[] Message_Request = new byte[1];
            byte[] Message_Length_In_Bytes = new byte[4];

            try
            {
                while (client.Connected)
                {
                    int bytesRead = stream.Read(Message_Request, 0, 1);
                    if (bytesRead == 0) break; // Client disconnected unexpectedly

                    switch ((MessageId)Message_Request[0])
                    {
                        case MessageId.LOG_IN:  // New user
                            {
                                stream.Read(Message_Length_In_Bytes, 0, 4);
                                int Message_Length = BitConverter.ToInt32(Message_Length_In_Bytes, 0);
                                Console.WriteLine("new connection");

                                byte[] current_username_in_bytes = new byte[Message_Length];
                                stream.Read(current_username_in_bytes, 0, Message_Length);
                                currentUsername = Encoding.UTF8.GetString(current_username_in_bytes);

                                int newUserId = _idManager.Generate_Random_User_ID(currentUsername);
                                User_Logs.AddItem(currentUsername, "0000-00-00-00:00:00");
                                Active_Connections[currentUsername] = stream;

                                List<byte> packetList = new List<byte>();

                                // send message saying accepted or rejected
                                if (User_Logs.Get_Item(currentUsername) != null)
                                {
                                    packetList.Add(0x01); // message type
                                    packetList.Add(0x01); // accepted

                                    packetList.Add((byte)newUserId);

                                    byte[] authenticate_Packet = packetList.ToArray();
                                    send_packet(stream, authenticate_Packet);
                                    BroadcastUserList();
                                }
                                else
                                {
                                    packetList.Add(0x01); // message type
                                    packetList.Add(0x02); // rejected

                                    byte[] authenticate_Packet = packetList.ToArray();
                                    send_packet(stream, authenticate_Packet);

                                    client.Close();
                                    return;
                                }
                            }
                            break;

                        case MessageId.GET_USERS:  // Give user list
                            {
                                byte[] requesterIdBuffer = new byte[1];
                                stream.Read(requesterIdBuffer, 0, 1);
                                byte requesterId = requesterIdBuffer[0];

                                byte[] finalOutgoingBytes = Send_User_List.GenerateUserListPacket(_idManager);
                                send_packet(stream, finalOutgoingBytes);
                            }
                            break;

                        case MessageId.SEND_MESSAGE:  // Send message
                            {
                                byte[] senderIdBuffer = new byte[1];
                                stream.Read(senderIdBuffer, 0, 1);
                                byte senderId = senderIdBuffer[0];

                                byte[] receiverIdBuffer = new byte[1];
                                stream.Read(receiverIdBuffer, 0, 1);
                                byte receiverId = receiverIdBuffer[0];

                                byte[] messageLengthBuffer = new byte[1];
                                stream.Read(messageLengthBuffer, 0, 1);
                                byte messageLength = messageLengthBuffer[0];

                                byte[] messageBytes = new byte[messageLength];
                                stream.Read(messageBytes, 0, messageLength);

                                if (_idManager.User_ID_Dictionary.TryGetValue(receiverId, out string receiverUsername))
                                {
                                    if (Active_Connections.TryGetValue(receiverUsername, out NetworkStream receiverStream))
                                    {
                                        byte[] outgoingMessage = new byte[3 + messageLength];
                                        outgoingMessage[0] = 0x03;
                                        outgoingMessage[1] = senderId;
                                        outgoingMessage[2] = messageLength;
                                        Array.Copy(messageBytes, 0, outgoingMessage, 3, messageLength);
                                        send_packet(receiverStream, outgoingMessage);
                                    }
                                    else
                                    {
                                        New_Message_Log.Add_New_Message($"[{DateTime.Now.ToString("yyyy-MM-dd-HH:mm:ss")}] {senderId} {receiverId} {Encoding.UTF8.GetString(messageBytes)}\n");
                                    }
                                }
                            }
                            break;

                        case MessageId.LOG_OUT:  // Log out
                            {
                                User_Logs.SetItem(currentUsername, DateTime.Now.ToString("yyyy-MM-dd-HH:mm:ss"));
                                Active_Connections.Remove(currentUsername);
                                client.Close();
                                return;
                            }

                        case MessageId.EXISTING_USER_LOG_IN:  // Existing user log in
                            {
                                stream.Read(Message_Length_In_Bytes, 0, 4);
                                int Message_Length = BitConverter.ToInt32(Message_Length_In_Bytes, 0);

                                byte[] current_username_in_bytes = new byte[Message_Length];
                                stream.Read(current_username_in_bytes, 0, Message_Length);
                                currentUsername = Encoding.UTF8.GetString(current_username_in_bytes);

                                Active_Connections[currentUsername] = stream;

                                foreach (var kvp in _idManager.User_ID_Dictionary)
                                {
                                    if (kvp.Value == currentUsername)
                                    {
                                        int loggedInUserId = kvp.Key;

                                        List<byte> authPacket = new List<byte>();
                                        authPacket.Add(0x01);                       // Message Type (Auth Response)
                                        authPacket.Add(0x01);                       // 0x01 = Accepted
                                        authPacket.Add((byte)loggedInUserId);       // Give them their ID back
                                        send_packet(stream, authPacket.ToArray());

                                        BroadcastUserList();

                                        break;
                                    }
                                }
                            }
                            break;

                        case MessageId.FETCH_OFFLINE_MESSAGES: // Client is asking for missed messages
                            {
                                byte[] idBuffer = new byte[1];
                                stream.Read(idBuffer, 0, 1);
                                byte requesterId = idBuffer[0]; // This is the user asking for their messages

                                List<string> offlineMessages = Check_Offline_Messages.Get_And_Remove_Messages(requesterId);

                                foreach (string rawFileLine in offlineMessages)
                                {
                                    string[] parts = rawFileLine.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                                    byte realSenderId = 0;
                                    int messageStartIndex = -1;


                                    for (int i = 0; i < parts.Length - 1; i++)
                                    {
                                        if (byte.TryParse(parts[i], out byte parsedSender) &&
                                            byte.TryParse(parts[i + 1], out byte parsedReceiver))
                                        {
                                            if (parsedReceiver == requesterId)
                                            {
                                                realSenderId = parsedSender;
                                                messageStartIndex = i + 2; // The message text starts immediately after the IDs
                                                break;
                                            }
                                        }
                                    }

                                    if (messageStartIndex != -1)
                                    {   
                                        string actualMessage = "";
                                        for (int i = messageStartIndex; i < parts.Length; i++)
                                        {
                                            actualMessage += parts[i] + (i == parts.Length - 1 ? "" : " ");
                                        }

                                        byte[] msgBytes = Encoding.UTF8.GetBytes(actualMessage);
                                        int length = msgBytes.Length;
                                        if (length > 255) length = 255;

                                        byte[] offlineOut = new byte[3 + length];
                                        offlineOut[0] = 0x03; // Standard Chat Message type

                                        offlineOut[1] = realSenderId;

                                        offlineOut[2] = (byte)length;
                                        Array.Copy(msgBytes, 0, offlineOut, 3, length);

                                        send_packet(stream, offlineOut);
                                    }
                                }
                                break;
                            }
                    }
                }
            }
            catch (Exception)
            {
                // Ensures a sudden disconnect doesn't crash the server
                Active_Connections.Remove(currentUsername);
            }
        }

        public static void Main()
        {
            int port = 5000;
            Random_User_ID masterIdManager = new Random_User_ID();

            TCPServer server = new TCPServer(port, masterIdManager);

            server.Start();
        }
    }
}