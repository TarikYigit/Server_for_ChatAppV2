using ServerForChatApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerForChatApp.Messages.ClientToServer
{
    internal class SendMessageRequestFromClient
    {
        public byte Sender { get; private set; }
        public byte Reciever { get; private set; }
        public byte[] Message { get; private set; }

        public SendMessageRequestFromClient(byte[] payload, RandomUserID idManager, UserDictionary userLogs)
        {
            if (payload != null && payload.Length > 2)
            {
                Sender = payload[0];
                Reciever = payload[1];
                Message = new byte[payload.Length - 2];
                Array.Copy(payload, 2, Message, 0, Message.Length);
            }
            else
            {
                throw new ArgumentException("Payload is invalid or too short.");
            }
        }
    }
}
