using System;
using System.Collections.Generic;
using System.Text;

namespace ServerForChatApp.Messages.ClientToServer
{
    internal class FetchOfflineMessagesForClient
    {
        public byte RequesterId { get; private set; }
        public List<byte[]> ReadyToSendPayloads { get; private set; } = new List<byte[]>();

        public FetchOfflineMessagesForClient(byte[] payload)
        {
            RequesterId = payload[0];

            List<string> rawOfflineMessages = CheckOfflineMessages.GetAndRemoveMessages(RequesterId);

            foreach (string line in rawOfflineMessages)
            {
                string[] parts = line.Split(new char[] { ' ' }, 4, StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length == 4 && byte.TryParse(parts[1], out byte senderId))
                {
                    byte[] msgBytes = Encoding.UTF8.GetBytes(parts[3]);

                    byte[] networkPayload = new byte[1 + msgBytes.Length];
                    networkPayload[0] = senderId;
                    Array.Copy(msgBytes, 0, networkPayload, 1, msgBytes.Length);

                    ReadyToSendPayloads.Add(networkPayload);
                }
            }
        }

        public byte GetId()
        {
            return (byte)MessageId.SEND_MESSAGE;
        }
    }
}