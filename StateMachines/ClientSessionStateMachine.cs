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

            private TCPServer myServer;

            private NetworkStream myStream;

            public ClientSession(TCPServer server, NetworkStream stream)
            {

                myServer = server;

                myStream = stream;

            }


            public INetworkMessage UpdateState(IRequest request)
            {
                switch (currentState)
                {

                    case LogState.NotLoggedIn:
                        switch (IRequest)
                        {

                        }
                        break;
                        if (action == MessageId.LOG_IN)
                        {

                            LoginRequest req = (LoginRequest)parsedRequest;

                            UserInfo existingUser = myServer.Users.GetUserByName(req.Username);

                            if (existingUser == null)
                            {

                                UserInfo newUser = myServer.Users.CreateAndAddUser(req.Username);

                                CurrentUserId = newUser.ID;

                                myServer.Connections.AddConnection(newUser.ID, myStream);

                                currentState = LogState.LoggedIn; // state transition

                                return new LoginResponse(newUser.ID, true);

                            }
                            return new LoginResponse(0, false);

                        }
                        else if (action == MessageId.EXISTING_USER_LOG_IN)
                        {

                            ExistingUserLogInRequest req = (ExistingUserLogInRequest)parsedRequest;

                            string requestedName = req.GetUsername();

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


                    case LogState.LoggedIn:
                        if (action == MessageId.GET_USERS)
                        {

                            GetUserListRequest req = (GetUserListRequest)parsedRequest;

                            List<UserInfo> userList = myServer.Users.GetAllUsersExcept(req.RequesterId);

                            return new GetUserListResponse(userList);

                        }
                        else if (action == MessageId.SEND_MESSAGE)
                        {

                            MessageDataGet req = (MessageDataGet)parsedRequest;

                            if (myServer.Users.GetUserById(req.GetReceiverId()) != null)
                            {

                                MessageSendNowRequest routingRequest = new MessageSendNowRequest(req.GetReceiverId(), myServer.Connections);

                                MessageResponse formattedMessage = new MessageResponse(req);

                                if (routingRequest.SendNow)
                                {
                                    Console.WriteLine("online");

                                    SendPacketClass.Send(req.GetReceiverId(), formattedMessage.GetId(), formattedMessage.ToBytes(), myServer.Connections);

                                }
                                else
                                {
                                    Console.WriteLine("storage");
                                    myServer.OfflineStorage.AddNewMessageForUser((byte)req.GetSenderId(), (byte)req.GetReceiverId(), formattedMessage.ToBytes());

                                }
                            }
                            return null; 
                        }
                        else if (action == MessageId.FETCH_OFFLINE_MESSAGES)
                        {

                            FetchOfflineMessageRequest request = (FetchOfflineMessageRequest)parsedRequest;

                            List<byte[]> messages = myServer.OfflineStorage.GetOfflineMessagesForUser(request.RequesterId);

                            return messages; 
                        }
                        break;
                }

                return null;

            }
        }
    }
}
