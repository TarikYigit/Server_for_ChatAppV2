using Server_for_ChatApp;
using Server_for_ChatApp.ConnectionManagers;
using Server_for_ChatApp.Interfaces;
using Server_for_ChatApp.Messages.ClientToServer;
using Server_for_ChatApp.Messages.ServerInternals;
using Server_for_ChatApp.Messages.ServerToClient;
using Server_for_ChatApp.UserManagers;
using Server_for_ChatApp.Vault;
using ServerForChatApp.Messages.ClientToServer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using static Server_for_ChatApp.StateMachines.ClientSessionStateMachine;
using static System.Collections.Specialized.BitVector32;

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

    enum LogState : int
    {

        LoggedIn = 1,

        NotLoggedIn = 0,

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
        {
            LogState currentState = LogState.NotLoggedIn;

            NetworkStream stream = client.GetStream();

            byte[] headerBuffer = new byte[5];

            ClientSession session = new ClientSession(this, stream);

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

                                LoginRequest request = new LoginRequest(payload);

                                LoginResponse response = (LoginResponse)session.UpdateState(messageId, request);

                                if (response != null)
                                {

                                    SendPacketClass.Send(stream, response.GetId(), response.ToBytes());

                                    BroadcastUserList();

                                }
                            }
                            break;

                        case MessageId.EXISTING_USER_LOG_IN:
                            {

                                ExistingUserLogInRequest request = new ExistingUserLogInRequest(payload);

                                ExistingUserLogInResponse response = (ExistingUserLogInResponse)session.UpdateState(messageId, request);

                                if (response != null)
                                {

                                    SendPacketClass.Send(stream, response.GetId(), response.ToBytes());

                                    BroadcastUserList();

                                }
                            }
                            break;

                        case MessageId.GET_USERS:
                            {

                                GetUserListRequest request = new GetUserListRequest(payload);

                                INetworkMessage response = session.UpdateState(messageId, request);

                                if (response != null)
                                {

                                    SendPacketClass.Send(request.RequesterId, response.GetId(), response.ToBytes(), Connections);

                                }
                            }
                            break;

                        case MessageId.SEND_MESSAGE:
                            {

                                MessageDataGet request = new MessageDataGet(payload);
                                
                                session.UpdateState(messageId, request);

                            }
                            break;

                        case MessageId.FETCH_OFFLINE_MESSAGES:
                            {

                                FetchOfflineMessageRequest request = new FetchOfflineMessageRequest(payload);

                                List<byte[]> offlineMessages = (List<byte[]>)session.UpdateState(messageId, request);

                                if (offlineMessages != null)
                                {

                                    foreach (byte[] messagePayload in offlineMessages)
                                    {

                                        SendPacketClass.Send(stream, (byte)MessageId.SEND_MESSAGE, messagePayload);

                                    }

                                    OfflineStorage.ClearOfflineMessagesForUser(request.RequesterId);


                                }
                            }
                            break;
                    }
                }
            }
            catch (Exception)
            {

                if (session.CurrentUserId != 0)
                {

                    Connections.RemoveConnection(session.CurrentUserId);

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