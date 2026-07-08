using Server_for_ChatApp.Interfaces;
using Server_for_ChatApp.Messages.ServerInternals;
using ServerForChatApp;
using System;
using System.IO;
using System.Text;

namespace Server_for_ChatApp.Messages.ServerToClient
{
    internal class MessageResponse : INetworkMessage
    {
        private MessageDataGet _messageData;

        public MessageResponse(MessageDataGet messageData)
        {

            _messageData = messageData;

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

                writer.Write(_messageData.GetSenderId());

                writer.Write(_messageData.GetMessageBytes());

                return ms.ToArray();

            }
        }
    }
}