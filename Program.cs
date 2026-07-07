using Server_for_ChatApp;
using Server_for_ChatApp.Vault;
using Server_for_ChatApp.ConnectionManagers;
using Server_for_ChatApp.Messages.ClientToServer;
using Server_for_ChatApp.Messages.ServerInternals;
using Server_for_ChatApp.Messages.ServerToClient;
using Server_for_ChatApp.UserManagers;
using ServerForChatApp.Messages.ClientToServer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using Server_for_ChatApp.Interfaces;

namespace ServerForChatApp
{
    enum MessageId : byte
    {

        LOG_IN = 1,

        GET_USERS = 2,

        SEND_MESSAGE = 3,

        EXISTING_USER_LOG_IN = 5,

        FETCH_OFFLINE_MESSAGES = 6,

    }

    class TCPServer
    {

        private TcpListener listener;

        private bool isRunning;

        public UserManager Users;

        public ConnectionManager Connections;

        public Dictionary<string, NetworkStream> ActiveConnections = new Dictionary<string, NetworkStream>();

        public IOfflineMessageStorage OfflineStorage = new PermanentOfflineMessageStorage();

        public TCPServer(int port, UserManager userManager)
        {

            listener = new TcpListener(IPAddress.Loopback, port);

            this.Users = userManager;

            this.Connections = new ConnectionManager();

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
            List<UserInfo> allUsers = Users.GetAllUsers();

            foreach (UserInfo targetUser in allUsers)
            {
                if (Connections.IsUserOnline(targetUser.ID))
                {
                    NetworkStream targetStream = Connections.GetStream(targetUser.ID);

                    List<UserInfo> otherUsers = Users.GetAllUsersExcept(targetUser.ID);

                    GetUserListResponse listResponse = new GetUserListResponse(otherUsers);

                    try
                    {
                        SendPacketClass.Send(targetStream, listResponse);
                    }
                    catch (Exception)
                    {

                    }
                }
            }
        }

        public void HandleClient(TcpClient client)
        {   //state machine here

            NetworkStream stream = client.GetStream();

            int currentUserId = 0;

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

                                LoginRequest loginRequest = new LoginRequest(payload);

                                // State makinesine ExecuteMessage(request);

                                UserInfo existingUser = Users.GetUserByName(loginRequest.Username);


                                if (existingUser == null)
                                {

                                    UserInfo newUser = Users.CreateAndAddUser(loginRequest.Username);

                                    currentUserId = newUser.ID;

                                    Connections.AddConnection(newUser.ID, stream);

                                    LoginResponse loginResponse = new LoginResponse(newUser.ID, true);

                                    SendPacketClass.Send(newUser.ID, loginResponse.GetId(), loginResponse.ToBytes(), Connections);

                                    BroadcastUserList();

                                }
                                else
                                {

                                    LoginResponse loginResponse = new LoginResponse(0, false);

                                    SendPacketClass.Send(stream, loginResponse.GetId(), loginResponse.ToBytes());

                                    client.Close();

                                    return;

                                }
                            }
                            break;

                        case MessageId.GET_USERS:
                            {

                                GetUserListRequest request = new GetUserListRequest(payload);

                                List<UserInfo> UserList = Users.GetAllUsersExcept(request.RequesterId);

                                GetUserListResponse response = new GetUserListResponse(UserList);

                                int myRequest = request.RequesterId;

                                SendPacketClass.Send( myRequest , response.GetId() , response.ToBytes() , Connections );

                            }
                            break;

                        case MessageId.SEND_MESSAGE:
                            {

                                MessageDataGet messageData = new MessageDataGet(payload);

                                if (!(Users.GetUserById(messageData.GetReceiverId()) == null))
                                {

                                    MessageSendNowRequest routingRequest = new MessageSendNowRequest(messageData.GetReceiverId(), Connections);

                                    if (routingRequest.SendNow)
                                    {

                                        MessageResponse formattedMessage = new MessageResponse(messageData);

                                        INetworkMessage message = formattedMessage;

                                        SendPacketClass.Send(messageData.GetReceiverId(), formattedMessage.GetId(), formattedMessage.ToBytes(), Connections);

                                    }
                                    else
                                    {

                                        MessageResponse formattedMessage = new MessageResponse(messageData);

                                        OfflineStorage.AddNewMessageForUser((byte)messageData.GetSenderId(), (byte)messageData.GetReceiverId(), formattedMessage.ToBytes());
                                    }

                                }
                                
                            }
                            break;


                        case MessageId.EXISTING_USER_LOG_IN:
                            {

                                ExistingUserLogInRequest existingUserLoginRequest = new ExistingUserLogInRequest(payload);

                                UserInfo existingUser = Users.GetUserByName(existingUserLoginRequest.GetUsername());

                                if (existingUser != null)
                                {
                                    currentUserId = existingUser.ID;

                                    ExistingUserLogInResponse existing = new ExistingUserLogInResponse(existingUser.username, Users, true);

                                    Connections.AddConnection(existingUser.ID, stream);

                                    SendPacketClass.Send(existingUser.ID, existing.GetId(), existing.ToBytes(), Connections);

                                    BroadcastUserList();

                                }
                                else
                                {

                                    ExistingUserLogInResponse notExisting = new ExistingUserLogInResponse(existingUser.username, Users, false);

                                    SendPacketClass.Send(stream, notExisting.GetId(), notExisting.ToBytes());

                                    client.Close();

                                    return;
                                }
                            }
                            break;

                        case MessageId.FETCH_OFFLINE_MESSAGES:
                            {

                                FetchOfflineMessageRequest fetch = new FetchOfflineMessageRequest(payload);

                                List<byte[]> offlineMessages = OfflineStorage.GetOfflineMessagesForUser(fetch.RequesterId);

                                foreach (byte[] msgPayload in offlineMessages)
                                {

                                    SendPacketClass.Send(stream, (byte)MessageId.SEND_MESSAGE, msgPayload);

                                }

                                OfflineStorage.ClearOfflineMessagesForUser(fetch.RequesterId);
                            }
                            break;

                    }
                }
            }
            catch (Exception)
            {

                if (currentUserId != 0)
                {
                    Connections.RemoveConnection(currentUserId);

                    BroadcastUserList();
                }

            }
        }


        public static void Main()
        {

            int port = 5000;

            UserManager masterUserManager = new UserManager();

            TCPServer server = new TCPServer(port, masterUserManager);

            server.Start();

        }
    }
}