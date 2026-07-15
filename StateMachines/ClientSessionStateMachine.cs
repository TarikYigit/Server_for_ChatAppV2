using Server_for_ChatApp.Interfaces;
using Server_for_ChatApp.Interfaces.RequestInterfaces;
using Server_for_ChatApp.Managers.GroupChatManager;
using Server_for_ChatApp.Managers.UserManagers;
using Server_for_ChatApp.Messages.ClientToServer;
using Server_for_ChatApp.Messages.ServerToClient;
using ServerForChatApp;
using ServerForChatApp.Messages.ClientToServer;
using System.Net.Sockets;

namespace Server_for_ChatApp.StateMachines
{
    internal class ClientSessionStateMachine
    {
        public class ClientSession
        {
            public LogState CurrentState { get; private set; } = LogState.NotLoggedIn;

            private LogState currentState = LogState.NotLoggedIn;

            public int CurrentUserId { get; private set; } = 0;

            private readonly IUsers _users;

            private IOfflineMessageStorage _offlineMessageStorage;

            private readonly IConnections _connections;

            private IGroupChat _groupManager;

            private NetworkStream myStream;

            public ClientSession(IUsers users, IConnections connections, IOfflineMessageStorage offlineMessageStorage, NetworkStream stream, IGroupChat groupChat)
            {

                _users = users;

                _connections = connections;

                myStream = stream;

                _offlineMessageStorage = offlineMessageStorage;

                _groupManager = groupChat;

            }


            public INetworkMessage? ExecuteRequest(IRequest request)
            {
                MessageId action = (MessageId)request.GetId();

                switch (currentState)
                {

                    case LogState.NotLoggedIn:
                        switch (action)
                        {
                            case MessageId.REGISTER:
                                {

                                    RegisterRequest myRequest = (RegisterRequest)request;

                                    UserInfo existingUser = _users.GetUserByName(myRequest.GetUsername());

                                    bool IsPasswordStrong = PasswordManager.IsPasswordStrong(myRequest.GetPassword());

                                    if (existingUser != null)
                                    {

                                        return new RegisterResponse(0, false, true);

                                    }
                                    if (!IsPasswordStrong)
                                    {

                                        return new RegisterResponse(0, false, false);

                                    }
                                    UserInfo newUser = _users.CreateAndAddUser(myRequest.GetUsername(), myRequest.GetPassword());

                                    CurrentUserId = newUser.ID;

                                    _connections.AddConnection(newUser.ID, myStream);

                                    currentState = LogState.LoggedIn; // state transition

                                    ServerLogger.LogAuth($"User '{myRequest.GetUsername()}' authenticated securely. Assigned User ID: {newUser.ID}");

                                    return new RegisterResponse(newUser.ID, true, true);

                                }

                            case MessageId.LOGIN:
                                {

                                    LoginRequest myRequest = (LoginRequest)request;

                                    string requestedName = myRequest.GetUsername();

                                    string providedPassword = myRequest.GetPassword();

                                    UserInfo existingUser = _users.GetUserByName(requestedName);

                                    if (existingUser != null && _users.VerifyUserPassword(myRequest.GetUsername(), myRequest.GetPassword()))
                                    {

                                        CurrentUserId = existingUser.ID;

                                        _connections.AddConnection(existingUser.ID, myStream);

                                        currentState = LogState.LoggedIn; // state transition

                                        return new LoginResponse(requestedName, existingUser.ID , true);

                                    }
                                    return new LoginResponse(requestedName, 0, false);

                                }

                            case MessageId.LOG_OUT:
                                {
                                    //no response is sent to client for this message type
                                    return null;

                                }

                            case MessageId.GET_USERS:
                                {

                                    GetUserListRequest myRequest = (GetUserListRequest)request;

                                    List<UserInfo> userList = new();

                                    List<int> ints = new List<int>();

                                    return new GetUserListResponse(userList,ints);

                                }

                            case MessageId.SEND_MESSAGE:
                                {

                                    //no response is sent to client for this message type
                                    return null;

                                }

                            case MessageId.FETCH_OFFLINE_MESSAGES:
                                {

                                    //no response is sent to client for this message type
                                    return null;

                                }

                            case MessageId.CREATE_GROUP:
                                {

                                    //no response is sent to client for this message type
                                    return null;

                                }

                            case MessageId.GROUP_CHAT_MESSAGE:
                                {

                                    //no response is sent to client for this message type
                                    return null;

                                }

                            case MessageId.GROUP_LIST:
                                {

                                    //no response is sent to client for this message type
                                    return null;

                                }
                            case MessageId.LEAVE_GROUP:
                                {

                                    //no response is sent to client for this message type
                                    return null;

                                }

                            case MessageId.ADD_USER_TO_GROUP:
                                {

                                    //no response is sent to client for this message type
                                    return null;

                                }

                        }
                        break;

                    case LogState.LoggedIn:
                        switch (action)
                        {
                            case MessageId.REGISTER:
                                {

                                    return new RegisterResponse(0, false, false);

                                }

                            case MessageId.LOGIN:
                                {

                                    LoginRequest myRequest = (LoginRequest)request;

                                    string requestedName = myRequest.GetUsername();

                                    return new LoginResponse(requestedName, 0, false);

                                }

                            case MessageId.LOG_OUT:
                                {

                                    currentState = LogState.NotLoggedIn; // state transition

                                    return null;

                                }

                            case MessageId.GET_USERS:
                                {

                                    GetUserListRequest myRequest = (GetUserListRequest)request;

                                    List<UserInfo> userList = _users.GetAllUsersExcept(myRequest.GetUserID());

                                    List<int> activeList = new List<int>();

                                    foreach (UserInfo user in userList)
                                    {

                                        if (_connections.IsUserOnline(user.ID))
                                        {

                                            activeList.Add(user.ID);

                                        }
                                    }

                                    return new GetUserListResponse(userList, activeList);

                                }

                            case MessageId.SEND_MESSAGE:
                                {
                                    SendMessageRequest myRequest = (SendMessageRequest)request;

                                    byte receiverId = (byte)myRequest.GetReceiverId();

                                    byte senderId = (byte)myRequest.GetSenderId();

                                    if (_users.GetUserById(receiverId) != null)
                                    {
                                        MessageResponse formattedMessage = new MessageResponse(myRequest);

                                        if (_connections.IsUserOnline(receiverId))
                                        {

                                            NetworkStream targetStream = _connections.GetStream(receiverId);

                                            ConnectionManager.Send(formattedMessage.GetId(), formattedMessage.ToBytes(), targetStream);

                                        }
                                        else
                                        {

                                            _offlineMessageStorage.AddNewMessageForUser(senderId, receiverId, formattedMessage.ToBytes());

                                        }
                                    }

                                    return null;
                                }

                            case MessageId.FETCH_OFFLINE_MESSAGES:
                                {

                                    FetchOfflineMessageRequest myRequest = (FetchOfflineMessageRequest)request;

                                    byte targetUserId = (byte)myRequest.GetUserID();

                                    if (_connections.IsUserOnline(targetUserId))
                                    {

                                        NetworkStream targetStream = _connections.GetStream(targetUserId);

                                        List<byte[]> messages = _offlineMessageStorage.GetOfflineMessagesForUser(targetUserId);

                                        if (messages.Count > 0)
                                        {
                                            ServerLogger.LogVault($"Flushing {messages.Count} pending 1-on-1 messages to User {targetUserId}.");

                                            foreach (byte[] messagePayload in messages)
                                            {

                                                ConnectionManager.Send((byte)MessageId.SEND_MESSAGE, messagePayload, targetStream);

                                            }
                                            _offlineMessageStorage.ClearOfflineMessagesForUser(targetUserId);
                                        }

                                        List<byte[]> groupMessages = _offlineMessageStorage.GetOfflineGroupMessagesForUser(targetUserId);

                                        if (groupMessages.Count > 0)
                                        {
                                            ServerLogger.LogVault($"Flushing {groupMessages.Count} pending group messages to User {targetUserId}.");

                                            foreach (byte[] groupPayload in groupMessages)
                                            {

                                                ConnectionManager.Send((byte)MessageId.GROUP_CHAT_MESSAGE, groupPayload, targetStream);

                                            }
                                            _offlineMessageStorage.ClearOfflineGroupMessagesForUser(targetUserId);
                                        }
                                    }
                                    return null;
                                }

                            case MessageId.CREATE_GROUP:
                                {
                                    CreateGroupRequest myRequest = (CreateGroupRequest)request;

                                    List<int> usersToInvite = myRequest.UserIdsToInvite;


                                    if (!usersToInvite.Contains(CurrentUserId))
                                    {

                                        usersToInvite.Add(CurrentUserId);

                                    }

                                    GroupChatInfo newGroup = _groupManager.CreateGroupChat(myRequest.GroupName, usersToInvite);

                                    return null;

                                }
                            case MessageId.GROUP_CHAT_MESSAGE:
                                {
                                    GroupChatMessageRequest myRequest = (GroupChatMessageRequest)request;

                                    byte senderId = myRequest.SenderId;

                                    byte groupId = myRequest.GroupId;

                                    GroupChatInfo group = _groupManager.GetGroupById(groupId);

                                    if (group != null)
                                    {
                                        GroupMessageResponse formattedMessage = new GroupMessageResponse(senderId, groupId, myRequest.MessageBytes);

                                        byte[] finalPayload = formattedMessage.ToBytes();

                                        foreach (int memberId in group.GroupChatUsers)
                                        {
                                            if (memberId == senderId) continue;
                                                
                                            if (_connections.IsUserOnline(memberId))
                                            {

                                                ServerLogger.LogRoute($"Active stream found. Bouncing Group {groupId} msg to User {memberId}.");

                                                NetworkStream targetStream = _connections.GetStream(memberId);

                                                ConnectionManager.Send(formattedMessage.GetId(), finalPayload, targetStream);

                                            }
                                            else
                                            {

                                                ServerLogger.LogVault($"User {memberId} is offline. Dropping {finalPayload.Length} bytes to local vault.");

                                                _offlineMessageStorage.AddOfflineGroupMessage((byte)memberId, finalPayload);

                                            }

                                        }
                                    }
                                    return null;
                                }

                            case MessageId.GROUP_LIST:
                                {
                                    GroupListRequest myRequest = (GroupListRequest)request;

                                    if (myRequest.RequestedUserId != CurrentUserId) //cross referance to ensure more security since the packet might be altered
                                    {

                                        return null; 

                                    }

                                    List<GroupChatInfo> myGroups = _groupManager.GetGroupsForUser(CurrentUserId);

                                    return new GroupListResponse(myGroups);
                                }

                            case MessageId.LEAVE_GROUP:
                                {
                                    LeaveGroupRequest myRequest = (LeaveGroupRequest)request;

                                    _groupManager.RemoveUserFromGroup(myRequest.GroupId, CurrentUserId);

                                    List<GroupChatInfo> updatedGroups = _groupManager.GetGroupsForUser(CurrentUserId);

                                    return new GroupListResponse(updatedGroups);
                                }

                            case MessageId.ADD_USER_TO_GROUP:
                                {

                                    AddUserToGroupRequest myRequest = (AddUserToGroupRequest)request;

                                    _groupManager.AddUserToGroup(myRequest.GroupId, myRequest.UserToAddId);

                                    return null; 

                                }
                        }
                        break;
                }
                return null;
            }
        }
    }
}
