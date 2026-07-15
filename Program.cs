using Server_for_ChatApp.GroupChatManager;
using Server_for_ChatApp.Interfaces;
using Server_for_ChatApp.Interfaces.RequestInterfaces;
using Server_for_ChatApp.Managers.DatabaseManager;
using Server_for_ChatApp.Managers.GroupChatManager;
using Server_for_ChatApp.Managers.UserManagers;
using Server_for_ChatApp.Messages.ClientToServer;
using Server_for_ChatApp.Messages.ServerToClient;
using Server_for_ChatApp.Vault;
using ServerForChatApp.Messages.ClientToServer;
using System.Net;
using System.Net.Sockets;
using static Server_for_ChatApp.StateMachines.ClientSessionStateMachine;

namespace ServerForChatApp
{
    enum MessageId : byte
    {

        REGISTER = 1,

        GET_USERS = 2,

        SEND_MESSAGE = 3,

        LOG_OUT = 4,

        LOGIN = 5,

        FETCH_OFFLINE_MESSAGES = 6,

        CREATE_GROUP = 7,

        GROUP_CHAT_MESSAGE = 8,

        GROUP_LIST = 9,

        LEAVE_GROUP = 10,

        ADD_USER_TO_GROUP = 11,

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

        public IOfflineMessageStorage OfflineStorage = new PermanentOfflineMessageStorage();

        public IGroupChat GroupManager;

        public TCPServer(int port, UserManager userManager, IGroupChat groupManager)
        {

            listener = new TcpListener(IPAddress.Loopback, port);

            this.Users = userManager;

            this.Connections = new ConnectionManager();

            this.GroupManager = groupManager;
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

            List<int> onlineUserIds = new List<int>();

            foreach (var user in allUsers)
            {

                if (Connections.IsUserOnline(user.ID))
                {

                    onlineUserIds.Add(user.ID);

                }
            }

            foreach (UserInfo targetUser in allUsers)
            {
                if (Connections.IsUserOnline(targetUser.ID))
                {

                    NetworkStream targetStream = Connections.GetStream(targetUser.ID);

                    List<UserInfo> otherUsers = Users.GetAllUsersExcept(targetUser.ID);


                    GetUserListResponse listResponse = new GetUserListResponse(otherUsers, onlineUserIds);

                    try
                    {

                        ConnectionManager.Send(targetStream, listResponse);

                    }
                    catch (Exception)
                    {

                    }
                }
            }
        }

        public void BroadcastGroupList(int userId)
        {
            if (Connections.IsUserOnline(userId))
            {
                List<GroupChatInfo> userGroups = GroupManager.GetGroupsForUser(userId);

                GroupListGet listResponse = new GroupListGet(userGroups);

                try
                {

                    NetworkStream targetStream = Connections.GetStream(userId);

                    ConnectionManager.Send(listResponse.GetId(), listResponse.ToBytes(), targetStream);

                }
                catch (Exception)
                {
                }
            }
        }

        public void HandleClient(TcpClient client)
        {
            LogState currentState = LogState.NotLoggedIn;

            NetworkStream stream = client.GetStream();

            byte[] headerBuffer = new byte[5];

            ClientSession session = new ClientSession(this.Users, this.Connections, this.OfflineStorage, stream, this.GroupManager);

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

                    ServerLogger.LogNetwork($"Received 0x{headerBuffer[0]:X2} ({(MessageId)headerBuffer[0]}) -> Payload: {payloadLength} bytes");

                    switch (messageId)
                    {
                        case MessageId.REGISTER:
                            {

                                RegisterRequest request = new RegisterRequest(payload);

                                RegisterResponse response = (RegisterResponse)session.ExecuteRequest(request);

                                ConnectionManager.Send(stream, response.GetId(), response.ToBytes());

                                BroadcastUserList();

                            }
                            break;

                        case MessageId.LOGIN:
                            {

                                LoginRequest request = new LoginRequest(payload);

                                LoginResponse response = (LoginResponse)session.ExecuteRequest(request);

                                ConnectionManager.Send(stream, response.GetId(), response.ToBytes());

                                BroadcastUserList();

                                BroadcastGroupList(session.CurrentUserId); 

                            }
                            break;

                        case MessageId.GET_USERS:
                            {

                                GetUserListRequest request = new GetUserListRequest(payload);

                                INetworkMessage response = session.ExecuteRequest(request);

                                NetworkStream targetStream = Connections.GetStream(request.GetUserID());

                                ConnectionManager.Send(response.GetId(), response.ToBytes(), targetStream);

                            }
                            break;

                        case MessageId.SEND_MESSAGE:
                            {

                                SendMessageRequest request = new SendMessageRequest(payload);
                                
                                session.ExecuteRequest(request);

                            }
                            break;

                        case MessageId.FETCH_OFFLINE_MESSAGES:
                            {

                                FetchOfflineMessageRequest request = new FetchOfflineMessageRequest(payload);

                                session.ExecuteRequest(request);

                            }
                            break;

                        case MessageId.CREATE_GROUP:
                            {

                                CreateGroupRequest request = new CreateGroupRequest(payload);

                                session.ExecuteRequest(request);

                                BroadcastGroupList(session.CurrentUserId);

                                foreach (int invitedId in request.UserIdsToInvite)
                                {

                                    if (invitedId != session.CurrentUserId)
                                    {

                                        BroadcastGroupList(invitedId);

                                    }
                                }
                            }
                            break;

                        case MessageId.GROUP_CHAT_MESSAGE:
                            {

                                GroupChatMessageRequest request = new GroupChatMessageRequest(payload);

                                session.ExecuteRequest(request);

                            }
                            break;

                        case MessageId.GROUP_LIST:
                            {
                                GroupListRequest request = new GroupListRequest(payload);

                                BroadcastGroupList(session.CurrentUserId);

                            }
                            break;

                        case MessageId.LEAVE_GROUP:
                            {
                                LeaveGroupRequest request = new LeaveGroupRequest(payload);

                                INetworkMessage response = session.ExecuteRequest(request);

                                if (response != null)
                                {
                                    ConnectionManager.Send(stream, response.GetId(), response.ToBytes());
                                }
                            }
                            break;

                        case MessageId.ADD_USER_TO_GROUP:
                            {

                                AddUserToGroupRequest request = new AddUserToGroupRequest(payload);

                                session.ExecuteRequest(request); 

                                BroadcastGroupList(request.UserToAddId);
                            }
                            break;
                    }
                }
            }
            catch (Exception)
            {

                ServerLogger.LogError($"Stream terminated for User {session.CurrentUserId}. ");

                LogOutRequest request = new LogOutRequest((byte)session.CurrentUserId);

                session.ExecuteRequest(request);

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

            DatabaseManager myDatabase = new DatabaseManager();

            UserManager masterUserManager = new UserManager(myDatabase);

            GroupChatManager groupManager = new GroupChatManager(myDatabase);

            TCPServer server = new TCPServer(port, masterUserManager, groupManager);

            server.Start();

        }
    }
}