using System.Net.Sockets;
using Server_for_ChatApp.ConnectionManagers;

namespace Server_for_ChatApp.Messages.ServerInternals
{
    internal class MessageSendNowRequest
    {

        public bool SendNow { get; private set; }

        public NetworkStream ReceiverStream { get; private set; }

        public MessageSendNowRequest(int receiverId, ConnectionManager connections)
        {

            SendNow = false;

            if (connections.IsUserOnline(receiverId))
            {

                SendNow = true;

            }
        }
    }
}