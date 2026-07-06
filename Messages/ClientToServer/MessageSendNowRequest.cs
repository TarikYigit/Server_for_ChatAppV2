using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Server_for_ChatApp.Messages.ClientToServer
{
    internal class MessageSendNowRequest
    {
        public bool SendNow { get; private set; }
        public NetworkStream ReceiverStream { get; private set; }

        public MessageSendNowRequest(bool messageValidInfo, Dictionary<string, NetworkStream> ActiveConnections, string receiverUsername)
        {

            SendNow = false;

            ReceiverStream = null;

            if (messageValidInfo && ActiveConnections.TryGetValue(receiverUsername, out NetworkStream stream))
            {

                SendNow = true;

                ReceiverStream = stream; 

            }
        }
    }
}