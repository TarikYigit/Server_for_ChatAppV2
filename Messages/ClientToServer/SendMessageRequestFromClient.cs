using ServerForChatApp;
using System;
using System.IO;
using System.Text;

namespace Server_for_ChatApp.Messages.ClientToServer
{

    internal class SendMessageRequestFromClient
    {

        public byte SenderId { get; private set; }
        public byte ReceiverId { get; private set; }
        public byte[] MessageBytes { get; private set; }

        public string ReceiverUsername { get; private set; }
        public bool IsReceiverValid { get; private set; }

        public SendMessageRequestFromClient(byte[] payload, RandomUserID idManager)
        {
            using (MemoryStream ms = new MemoryStream(payload))

            using (BinaryReader reader = new BinaryReader(ms))
            {

                SenderId = reader.ReadByte();

                ReceiverId = reader.ReadByte();

                int remainingBytes = (int)(ms.Length - ms.Position);

                MessageBytes = reader.ReadBytes(remainingBytes);

            }

            if (idManager.UserIDDictionary.TryGetValue(ReceiverId, out string receiverUsername))
            {

                ReceiverUsername = receiverUsername;

                IsReceiverValid = true;

            }
            else
            {

                IsReceiverValid = false;

            }
        }

        public byte GetId()
        {

            return (byte)MessageId.SEND_MESSAGE;

        }

        public byte[] ToBytes()
        {

            using (MemoryStream ms = new MemoryStream())

            using (BinaryWriter writer = new BinaryWriter(ms))
            {

                writer.Write(SenderId);

                writer.Write(MessageBytes);

                return ms.ToArray();

            }
        }



        public void SaveToOfflineVault()
        {

            string messageText = Encoding.UTF8.GetString(MessageBytes);

            string logEntry = $"[{DateTime.Now.ToString("yyyy-MM-dd-HH:mm:ss")}] {SenderId} {ReceiverId} {messageText}\n";

            NewMessageLog.AddNewMessage(logEntry);

        }
    }
}