using Server_for_ChatApp.Messages.ServerInternals;
using ServerForChatApp;
using ServerForChatApp.Messages;
using System;
using System.IO;
using System.Text;

namespace Server_for_ChatApp.Messages.ServerToClient
{
    internal class MakeMessageToBeSentToClient : INetworkMessage
    {
        private byte[] _finalPayload;
        private MessageDataGet _messageData;

        public MakeMessageToBeSentToClient(MessageDataGet messageData)
        {

            _messageData = messageData;

            using (MemoryStream ms = new MemoryStream())

            using (BinaryWriter writer = new BinaryWriter(ms))
            {

                writer.Write(_messageData.GetSenderId());

                writer.Write(_messageData.GetMessageBytes()); 

                _finalPayload = ms.ToArray();

            }
        }

        public byte GetId()
        {

            return (byte)MessageId.SEND_MESSAGE;

        }

        public byte[] ToBytes()
        {

            return _finalPayload;

        }

        public void SaveToOfflineVault()
        {

            string messageText = Encoding.UTF8.GetString(_messageData.GetMessageBytes());

            string logEntry = $"[{DateTime.Now.ToString("yyyy-MM-dd-HH:mm:ss")}] {_messageData.GetSenderId()} {_messageData.GetReceiverId()} {messageText}\n";

            NewMessageLog.AddNewMessage(logEntry);

        }
    }
}