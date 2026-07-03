using Server_for_ChatApp.Messages.ClientToServer;
using ServerForChatApp.Messages.ClientToServer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ServerForChatApp
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

        public RandomUserID _idManager;
        UserDictionary UserLogs = new UserDictionary();
        public Dictionary<string, NetworkStream> ActiveConnections = new Dictionary<string, NetworkStream>();

        byte accepted = 0x01;
        public TCPServer(int port, RandomUserID idManager)
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
            GetUserListForClient listData = new GetUserListForClient(new byte[0], _idManager);

            foreach (var activeStream in ActiveConnections.Values.ToList())
            {
                try
                {
                    SendPacket(activeStream, listData.GetId(), listData.ToBytes());
                }
                catch (Exception)
                {
                }
            }
        }


        public void SendPacket(NetworkStream stream, byte messageId, byte[] payload) 
        {
            int payloadLength = payload?.Length ?? 0;
            byte[] finalPacket = new byte[1 + 4 + payloadLength];

            finalPacket[0] = messageId;

            byte[] lengthBytes = BitConverter.GetBytes(payloadLength);
            Array.Copy(lengthBytes, 0, finalPacket, 1, 4);

            if (payloadLength > 0)
            {
                Array.Copy(payload, 0, finalPacket, 5, payloadLength);
            }

            stream.Write(finalPacket, 0, finalPacket.Length);
            stream.Flush();
        }

        public void HandleClient(TcpClient client)
        {
            NetworkStream stream = client.GetStream();
            string currentUsername = "";

            byte[] headerBuffer = new byte[5];

            try
            {
                while (client.Connected)
                {
                    int headerRead = 0;
                    while (headerRead < 5)
                    {
                        int read = stream.Read(headerBuffer, headerRead, 5 - headerRead);
                        if (read == 0) throw new Exception("Client disconnected");
                        headerRead += read;
                    }

                    MessageId messageId = (MessageId)headerBuffer[0];
                    int payloadLength = BitConverter.ToInt32(headerBuffer, 1);

                    byte[] payload = new byte[payloadLength];
                    int payloadRead = 0;
                    while (payloadRead < payloadLength)
                    {
                        int read = stream.Read(payload, payloadRead, payloadLength - payloadRead);
                        if (read == 0) throw new Exception("Client disconnected mid-payload");
                        payloadRead += read;
                    }

                    switch (messageId)
                    {
                        case MessageId.LOG_IN:
                            {
                                LoginForClient loginRequest = new LoginForClient(payload, _idManager, UserLogs);
                                SendPacket(stream, loginRequest.GetId(), loginRequest.ToBytes());                                

                                //Internal Server Logic, not sent to client
                                bool flowControl = ServerDictionariesHoldingClientActivity(client, stream, ref currentUsername, loginRequest);
                                if (!flowControl)
                                {
                                    return;
                                }
                            }
                            break;

                        case MessageId.GET_USERS:
                            {
                                GetUserListForClient getUserListForClient = new GetUserListForClient(payload, _idManager);

                                SendPacket(stream, getUserListForClient.GetId(), getUserListForClient.ToBytes());
                            }
                            break;

                        case MessageId.SEND_MESSAGE:
                            {
                                SendMessageRequestFromClient messageRequest = new SendMessageRequestFromClient(payload, _idManager);

                                if (messageRequest.IsReceiverValid)
                                {
                                    if (ActiveConnections.TryGetValue(messageRequest.ReceiverUsername, out NetworkStream receiverStream))
                                    {
                                        SendPacket(receiverStream, messageRequest.GetId(), messageRequest.ToBytes());
                                    }
                                    else
                                    {
                                        messageRequest.SaveToOfflineVault();
                                    }
                                }
                            }
                            break;

                        case MessageId.LOG_OUT:
                            {
                                ClientLoggedOutByPressingActualLogOut(client, currentUsername);
                                return;
                            }

                        case MessageId.EXISTING_USER_LOG_IN:
                            {
                                ExistingUserLogInRequest existingRequest = new ExistingUserLogInRequest(payload, _idManager);

                                if (existingRequest.IsValid)
                                {
                                    currentUsername = ActiveUserDataForServerUse(stream, existingRequest);
                                    SendPacket(stream, existingRequest.GetId(), existingRequest.ToBytes());
                                    BroadcastUserList();
                                }
                                else
                                {
                                    SendPacket(stream, existingRequest.GetId(), existingRequest.ToBytes());
                                    client.Close();
                                    return;
                                }
                            }
                            break;

                        case MessageId.FETCH_OFFLINE_MESSAGES:
                            {
                                FetchOfflineMessagesForClient fetchRequest = new FetchOfflineMessagesForClient(payload);

                                foreach (byte[] msgPayload in fetchRequest.ReadyToSendPayloads)
                                {
                                    SendPacket(stream, fetchRequest.GetId(), msgPayload);
                                }
                            }
                            break;
                    }
                }
            }
            catch (Exception)
            {
                ActiveConnections.Remove(currentUsername);
            }
        }

        private string ActiveUserDataForServerUse(NetworkStream stream, ExistingUserLogInRequest existingRequest)
        {
            string currentUsername = existingRequest.Username;
            ActiveConnections[currentUsername] = stream;
            return currentUsername;
        }

        private void ClientLoggedOutByPressingActualLogOut(TcpClient client, string currentUsername)
        {
            UserLogs.SetItem(currentUsername, DateTime.Now.ToString("yyyy-MM-dd-HH:mm:ss"));
            ActiveConnections.Remove(currentUsername);
            client.Close();
            return;
        }

        private bool ServerDictionariesHoldingClientActivity(TcpClient client, NetworkStream stream, ref string currentUsername, LoginForClient loginRequest)
        {
            if (loginRequest.IsAccepted)
            {
                currentUsername = loginRequest.Username;
                ActiveConnections[currentUsername] = stream;
                BroadcastUserList();
            }
            else
            {
                client.Close();
                return false;
            }

            return true;
        }

        public static void Main()
        {
            int port = 5000;
            RandomUserID masterIdManager = new RandomUserID();
            TCPServer server = new TCPServer(port, masterIdManager);
            server.Start();
        }
    }
}