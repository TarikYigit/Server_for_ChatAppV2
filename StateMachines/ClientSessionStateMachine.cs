using Server_for_ChatApp.ConnectionManagers;
using Server_for_ChatApp.Interfaces;
using Server_for_ChatApp.Interfaces.RequestInterfaces;
using Server_for_ChatApp.Messages.ClientToServer;
using Server_for_ChatApp.Messages.ServerInternals;
using Server_for_ChatApp.Messages.ServerToClient;
using Server_for_ChatApp.UserManagers;
using ServerForChatApp;
using ServerForChatApp.Messages.ClientToServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

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

            private NetworkStream myStream;

            public ClientSession(IUsers users, IConnections connections, IOfflineMessageStorage offlineMessageStorage, NetworkStream stream)
            {

                _users = users;

                _connections = connections;

                myStream = stream;

                _offlineMessageStorage = offlineMessageStorage;

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

                                    return new GetUserListResponse(userList);

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

                                    return new GetUserListResponse(userList);

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

                                    List<byte[]> messages = _offlineMessageStorage.GetOfflineMessagesForUser(targetUserId);

                                    if (messages.Count > 0 && _connections.IsUserOnline(targetUserId))
                                    {

                                        NetworkStream targetStream = _connections.GetStream(targetUserId);

                                        foreach (byte[] messagePayload in messages)
                                        {

                                            ConnectionManager.Send((byte)MessageId.SEND_MESSAGE, messagePayload, targetStream);

                                        }

                                        _offlineMessageStorage.ClearOfflineMessagesForUser(targetUserId);
                                    }

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
