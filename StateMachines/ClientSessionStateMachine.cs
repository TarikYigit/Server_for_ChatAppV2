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


            public INetworkMessage? UpdateState(IRequest request)
            {
                MessageId action = (MessageId)request.GetId();

                switch (currentState)
                {

                    case LogState.NotLoggedIn:
                        switch (action)
                        {
                            case MessageId.LOG_IN:
                                {

                                    LoginRequest myRequest = (LoginRequest)request;

                                    UserInfo existingUser = myServer.Users.GetUserByName(myRequest.Username);

                                    if (existingUser == null)
                                    {

                                        UserInfo newUser = myServer.Users.CreateAndAddUser(myRequest.Username);

                                        CurrentUserId = newUser.ID;

                                        myServer.Connections.AddConnection(newUser.ID, myStream);

                                        currentState = LogState.LoggedIn; // state transition

                                        return new LoginResponse(newUser.ID, true);

                                    }
                                    return new LoginResponse(0, false);
                                }
                                break;

                            case MessageId.EXISTING_USER_LOG_IN:
                                {

                                    ExistingUserLogInRequest myRequest = (ExistingUserLogInRequest)request;

                                    string requestedName = myRequest.GetUsername();

                                    UserInfo existingUser = myServer.Users.GetUserByName(requestedName);

                                    if (existingUser != null)
                                    {

                                        CurrentUserId = existingUser.ID;

                                        myServer.Connections.AddConnection(existingUser.ID, myStream);

                                        currentState = LogState.LoggedIn; // state transition

                                        return new ExistingUserLogInResponse(requestedName, myServer.Users, true);

                                    }
                                    return new ExistingUserLogInResponse(requestedName, myServer.Users, false);

                                }
                                break;

                            case MessageId.LOG_OUT:
                                {
                                    //no response is sent to client for this message type
                                    return null;

                                }
                                break;

                            case MessageId.GET_USERS:
                                {

                                    GetUserListRequest myRequest = (GetUserListRequest)request;

                                    List<UserInfo> userList = new();

                                    return new GetUserListResponse(userList);

                                }
                                break;

                            case MessageId.SEND_MESSAGE:
                                {

                                    //no response is sent to client for this message type
                                    return null;

                                }
                                break;

                            case MessageId.FETCH_OFFLINE_MESSAGES:
                                {

                                    //no response is sent to client for this message type
                                    return null;

                                }
                                break;

                        }
                        break;

                    case LogState.LoggedIn:
                        switch (action)
                        {
                            case MessageId.LOG_IN:
                                {

                                    return new LoginResponse(0, false);

                                }
                                break;

                            case MessageId.EXISTING_USER_LOG_IN:
                                {

                                    ExistingUserLogInRequest myRequest = (ExistingUserLogInRequest)request;

                                    string requestedName = myRequest.GetUsername();

                                    return new ExistingUserLogInResponse(requestedName, myServer.Users, false);

                                }
                                break;

                            case MessageId.LOG_OUT:
                                {

                                    currentState = LogState.NotLoggedIn; // state transition

                                    return null;

                                }
                                break;

                            case MessageId.GET_USERS:
                                {

                                    GetUserListRequest myRequest = (GetUserListRequest)request;

                                    List<UserInfo> userList = myServer.Users.GetAllUsersExcept(myRequest.GetUserID());

                                    return new GetUserListResponse(userList);

                                }
                                break;

                            case MessageId.SEND_MESSAGE:
                                {

                                    SendMessageRequest myRequest = (SendMessageRequest)request;

                                    if (myServer.Users.GetUserById(myRequest.GetReceiverId()) != null)
                                    {

                                        MessageSendNowRequest routingRequest = new MessageSendNowRequest(myRequest.GetReceiverId(), myServer.Connections);

                                        MessageResponse formattedMessage = new MessageResponse(myRequest);

                                        if (routingRequest.SendNow)
                                        {

                                            SendPacketClass.Send(myRequest.GetReceiverId(), formattedMessage.GetId(), formattedMessage.ToBytes(), myServer.Connections);

                                        }
                                        else
                                        {

                                            myServer.OfflineStorage.AddNewMessageForUser((byte)myRequest.GetSenderId(), (byte)myRequest.GetReceiverId(), formattedMessage.ToBytes());

                                        }
                                    }

                                    return null;
                                }
                                break;

                            case MessageId.FETCH_OFFLINE_MESSAGES:
                                {

                                    FetchOfflineMessageRequest myRequest = (FetchOfflineMessageRequest)request;

                                    List<byte[]> messages = myServer.OfflineStorage.GetOfflineMessagesForUser(myRequest.GetUserID());

                                    foreach (byte[] messagePayload in messages)
                                    {

                                        SendPacketClass.Send(myRequest.GetUserID(), (byte)MessageId.SEND_MESSAGE, messagePayload, myServer.Connections);

                                    }

                                    myServer.OfflineStorage.ClearOfflineMessagesForUser(myRequest.GetUserID());

                                    return null;

                                }
                                break;

                        }
                        break;
                }
                return null;
            }
        }
    }
}
