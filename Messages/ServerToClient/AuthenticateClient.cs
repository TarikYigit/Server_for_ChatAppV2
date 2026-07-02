using ServerForChatApp.Messages.ClientToServer;
using ServerForChatApp.Messages.ClientToServer;
using ServerForChatApp;
using System.Net.Sockets;

namespace ServerForChatApp.Messages.ServerToClient
{
    internal class AuthenticateClient
    {
        //accepts new user login requests
        public static void SendAuthenticationPacket(TCPServer server, NetworkStream stream, LoginForClient loginData)
        {
            SendAuthBytes(server, stream, loginData.IsAccepted, loginData.AssignedId);
        }

        //accepts existing user login requests
        public static void SendAuthenticationPacket(TCPServer server, NetworkStream stream, ExistingUserLogInRequest loginData)
        {
            SendAuthBytes(server, stream, loginData.IsValid, loginData.LoggedInUserId);
        }

        // authorize existing user or new user and send the response to the client
        private static void SendAuthBytes(TCPServer server, NetworkStream stream, bool isAccepted, int userId)
        {
            byte[] payload;
            if (isAccepted)
            {
                payload = new byte[] { 0x01, (byte)userId };
            }
            else
            {
                payload = new byte[] { 0x02 };
            }

            server.SendPacket(stream, 0x01, payload);
        }
    }
}