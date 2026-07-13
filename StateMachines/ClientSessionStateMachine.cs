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

            public TCPServer myServer;

            private NetworkStream myStream;

            public ClientSession(TCPServer server, NetworkStream stream)
            {

                myServer = server;

                myStream = stream;

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

                                    UserInfo existingUser = myServer.Users.GetUserByName(myRequest.GetUsername());

                                    bool IsPasswordStrong = PasswordManager.IsPasswordStrong(myRequest.GetPassword());

                                    if (existingUser != null)
                                    {

                                        return new RegisterResponse(0, false, true);

                                    }
                                    if (!IsPasswordStrong)
                                    {

                                        return new RegisterResponse(0, false, false);

                                    }
                                    UserInfo newUser = myServer.Users.CreateAndAddUser(myRequest.GetUsername(), myRequest.GetPassword());

                                    CurrentUserId = newUser.ID;

                                    myServer.Connections.AddConnection(newUser.ID, myStream);

                                    currentState = LogState.LoggedIn; // state transition

                                    return new RegisterResponse(newUser.ID, true, true);

                                }

                            case MessageId.LOGIN:
                                {

                                    LoginRequest myRequest = (LoginRequest)request;

                                    string requestedName = myRequest.GetUsername();

                                    string providedPassword = myRequest.GetPassword();

                                    UserInfo existingUser = myServer.Users.GetUserByName(requestedName);

                                    if (existingUser != null && myServer.Users.VerifyUserPassword(myRequest.GetUsername(), myRequest.GetPassword()))
                                    {

                                        CurrentUserId = existingUser.ID;

                                        myServer.Connections.AddConnection(existingUser.ID, myStream);

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

                                    List<UserInfo> userList = myServer.Users.GetAllUsersExcept(myRequest.GetUserID());

                                    return new GetUserListResponse(userList);

                                }

                            case MessageId.SEND_MESSAGE:
                                {

                                    SendMessageRequest myRequest = (SendMessageRequest)request;

                                    if (myServer.Users.GetUserById(myRequest.GetReceiverId()) != null)
                                    {

                                        MessageSendNowRequest routingRequest = new MessageSendNowRequest(myRequest.GetReceiverId(), myServer.Connections);

                                        MessageResponse formattedMessage = new MessageResponse(myRequest);

                                        if (routingRequest.SendNow)
                                        {



                                            ConnectionManager.Send(myRequest.GetReceiverId(), formattedMessage.GetId(), formattedMessage.ToBytes(), myServer.Connections);

                                        }
                                        else
                                        {

                                            myServer.OfflineStorage.AddNewMessageForUser((byte)myRequest.GetSenderId(), (byte)myRequest.GetReceiverId(), formattedMessage.ToBytes());

                                        }
                                    }

                                    return null;
                                }

                            case MessageId.FETCH_OFFLINE_MESSAGES:
                                {

                                    FetchOfflineMessageRequest myRequest = (FetchOfflineMessageRequest)request;

                                    List<byte[]> messages = myServer.OfflineStorage.GetOfflineMessagesForUser(myRequest.GetUserID());

                                    foreach (byte[] messagePayload in messages)
                                    {

                                        ConnectionManager.Send(myRequest.GetUserID(), (byte)MessageId.SEND_MESSAGE, messagePayload, myServer.Connections);

                                    }

                                    myServer.OfflineStorage.ClearOfflineMessagesForUser(myRequest.GetUserID());

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
