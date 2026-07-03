using ServerForChatApp;
using System;
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
            SenderId = payload[0];
            ReceiverId = payload[1];

            MessageBytes = new byte[payload.Length - 2];
            Array.Copy(payload, 2, MessageBytes, 0, MessageBytes.Length);

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
            byte[] outgoingPayload = new byte[1 + MessageBytes.Length];
            outgoingPayload[0] = SenderId;
            Array.Copy(MessageBytes, 0, outgoingPayload, 1, MessageBytes.Length);

            return outgoingPayload;
        }

        public void SaveToOfflineVault()
        {
            string messageText = Encoding.UTF8.GetString(MessageBytes);
            string logEntry = $"[{DateTime.Now.ToString("yyyy-MM-dd-HH:mm:ss")}] {SenderId} {ReceiverId} {messageText}\n";
            NewMessageLog.AddNewMessage(logEntry);
        }
    }
}