using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ServerForChatApp.Messages.ClientToServer
{

    internal class FetchOfflineMessagesForClient
    {

        public byte RequesterId { get; private set; }

        public List<byte[]> ReadyToSendPayloads { get; private set; } = new List<byte[]>();



        public FetchOfflineMessagesForClient(byte[] payload)
        {

            if (payload != null && payload.Length > 0)
            {

                using (MemoryStream payloadStream = new MemoryStream(payload))

                using (BinaryReader reader = new BinaryReader(payloadStream))
                {

                    RequesterId = reader.ReadByte();

                }
            }

            List<string> rawOfflineMessages = CheckOfflineMessages.GetAndRemoveMessages(RequesterId);


            foreach (string line in rawOfflineMessages)
            {

                string[] parts = line.Split(new char[] { ' ' }, 4, StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length == 4 && byte.TryParse(parts[1], out byte senderId))
                {

                    byte[] msgBytes = Encoding.UTF8.GetBytes(parts[3]);

                    using (MemoryStream ms = new MemoryStream())

                    using (BinaryWriter writer = new BinaryWriter(ms))
                    {

                        writer.Write(senderId);

                        writer.Write(msgBytes);

                        ReadyToSendPayloads.Add(ms.ToArray());

                    }
                }
            }
        }



        public byte GetId()
        {

            return (byte)MessageId.SEND_MESSAGE;

        }
    }
}