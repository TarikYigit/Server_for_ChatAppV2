using Server_for_ChatApp.Messages.ClientToServer;
using ServerForChatApp.Messages;
using ServerForChatApp.Messages.ClientToServer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using ServerForChatApp;

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

        public RandomUserID idManager;

        UserDictionary UserLogs = new UserDictionary();

        public Dictionary<string, NetworkStream> ActiveConnections = new Dictionary<string, NetworkStream>();

        public IOfflineMessageStorage OfflineStorage = new TemporaryOfflineMessageStorage();

        public TCPServer(int port, RandomUserID idManager)
        {

            listener = new TcpListener(IPAddress.Loopback, port);

            this.idManager = idManager;

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
            GetCopyOfUserDictionaryAll filterService = new GetCopyOfUserDictionaryAll();

            var connectionsSnapshot = ActiveConnections.ToList();

            foreach (var connection in connectionsSnapshot)
            {
                string targetUsername = connection.Key;
                NetworkStream targetStream = connection.Value;

                byte targetUserId = 0;

                lock (idManager.UserIDDictionary)
                {
                    foreach (var kvp in idManager.UserIDDictionary)
                    {
                        if (kvp.Value == targetUsername)
                        {
                            targetUserId = (byte)kvp.Key;
                            break;
                        }
                    }
                }

                Dictionary<int, string> filteredUsers = filterService.GetSafeFilteredUsers(idManager.UserIDDictionary, targetUserId);

                GetUserListResponse listResponse = new GetUserListResponse(filteredUsers);

                try
                {
                    INetworkMessage message = listResponse;
                    SendPacket(targetStream, message.GetId(), message.ToBytes());
                }
                catch (Exception)
                {
                }
            }
        }



        public void SendPacket(NetworkStream stream, byte messageId, byte[] payload)
        {

            int payloadLength = payload?.Length ?? 0;

            using (MemoryStream ms = new MemoryStream())

            using (BinaryWriter writer = new BinaryWriter(ms))
            {

                writer.Write(messageId);

                writer.Write(payloadLength); 

                if (payloadLength > 0)
                {

                    writer.Write(payload);

                }
                byte[] finalPacket = ms.ToArray();

                stream.Write(finalPacket, 0, finalPacket.Length);

                stream.Flush();

            }
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

                        if (read == 0) throw new Exception("Client disconnected during payload transfer");

                        payloadRead += read;

                    }


                    switch (messageId)
                    {
                        case MessageId.LOG_IN:
                            {

                                GetCopyOfUserDictionaryOnlyNames dictionaryCopier = new GetCopyOfUserDictionaryOnlyNames();

                                List<string> usernameList = dictionaryCopier.GetUsernames(UserLogs.UserLogData);

                                LoginRequest loginRequest = new LoginRequest(payload, usernameList);

                                LoginForClient loginResponse = new LoginForClient(payload, idManager, UserLogs, loginRequest.GetAccepted());

                                INetworkMessage message = loginResponse;

                                SendPacket(stream, message.GetId(), message.ToBytes());

                                //Internal Server Logic, not sent to client
                                bool flowControl = ServerDictionariesHoldingClientActivity(client, stream, ref currentUsername, loginResponse);

                                if (!flowControl)
                                {
                                        
                                    return;

                                }
                            }
                            break;

                        case MessageId.GET_USERS:
                            {

                                GetUserListRequest request = new GetUserListRequest(payload);

                                GetCopyOfUserDictionaryAll filterService = new GetCopyOfUserDictionaryAll();

                                Dictionary<int, string> filteredUsers = filterService.GetSafeFilteredUsers(idManager.UserIDDictionary, request.RequesterId);

                                GetUserListResponse response = new GetUserListResponse(filteredUsers);

                                INetworkMessage message = response;

                                SendPacket(stream, message.GetId(), message.ToBytes());

                            }
                            break;

                        case MessageId.SEND_MESSAGE:
                            {

                                MessageDataGet messageData = new MessageDataGet(payload);

                                GetCopyOfUserDictionaryAll filterService = new GetCopyOfUserDictionaryAll();

                                Dictionary<int, string> filteredUsers = filterService.GetSafeFilteredUsers(idManager.UserIDDictionary, messageData.GetSenderId());
    
                                MessageValid doesRecieverExist = new MessageValid(messageData.GetReceiverId(), filteredUsers);

                                MessageSendNowRequest routingRequest = new MessageSendNowRequest(doesRecieverExist.GetIsReceiverValid(), ActiveConnections, doesRecieverExist.GetReceiverUsername());

                                MakeMessageToBeSentToClient formattedMessage = new MakeMessageToBeSentToClient(messageData);

                                if (routingRequest.SendNow)
                                {

                                    INetworkMessage message = formattedMessage;

                                    SendPacket(routingRequest.ReceiverStream, message.GetId(), message.ToBytes());

                                }
                                else
                                {

                                    if (doesRecieverExist.GetIsReceiverValid())
                                    {

                                        OfflineStorage.AddNewMessageForUser(messageData.GetSenderId(), messageData.GetReceiverId(), messageData.GetMessageBytes());

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

                                ExistingUserLogInRequest existingUserLoginRequest = new ExistingUserLogInRequest(payload);

                                GetCopyOfUserDictionaryAll filterService = new GetCopyOfUserDictionaryAll();

                                Dictionary<int, string> filteredUsers = filterService.GetSafeFilteredUsers(idManager.UserIDDictionary, 00);

                                ExistingUserLogInResponse existingRequest = new ExistingUserLogInResponse(existingUserLoginRequest.GetUsername(), filteredUsers);

                                INetworkMessage message = existingRequest;

                                if (existingRequest.IsValid)
                                {

                                    currentUsername = ActiveUserDataForServerUse(stream, existingUserLoginRequest.GetUsername());

                                    SendPacket(stream, message.GetId(), message.ToBytes());

                                    BroadcastUserList();

                                }
                                else
                                {

                                    SendPacket(stream, message.GetId(), message.ToBytes());

                                    client.Close();

                                    return;
                                }
                            }
                            break;

                        case MessageId.FETCH_OFFLINE_MESSAGES:
                            {
                                FetchOfflineMessageRequest fetch = new FetchOfflineMessageRequest(payload);
                                byte userId = fetch.GetUserID();

                                List<byte[]> offlineMessages = OfflineStorage.GetOfflineMessagesForUser(userId);

                                foreach (byte[] msgPayload in offlineMessages)
                                {

                                    SendPacket(stream, (byte)MessageId.SEND_MESSAGE, msgPayload);

                                }

                                OfflineStorage.ClearOfflineMessagesForUser(userId);
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



        private string ActiveUserDataForServerUse(NetworkStream stream, string username)
        {

            ActiveConnections[username] = stream;

            return username;

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