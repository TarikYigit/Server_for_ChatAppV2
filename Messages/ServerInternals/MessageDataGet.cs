using ServerForChatApp;
using System.IO;

namespace Server_for_ChatApp.Messages.ServerInternals
{
    internal class MessageDataGet
    {
        private byte SenderId { get; set; }
        private byte ReceiverId { get; set; }
        private byte[] MessageBytes { get; set; }


        public MessageDataGet(byte[] payload)
        {
            using (MemoryStream ms = new MemoryStream(payload))

            using (BinaryReader reader = new BinaryReader(ms))
            {

                SenderId = reader.ReadByte();

                ReceiverId = reader.ReadByte();

                int remainingBytes = (int)(ms.Length - ms.Position);

                MessageBytes = reader.ReadBytes(remainingBytes);

            }
        }

        public byte GetSenderId() 
        { 

            return SenderId; 

        }

        public byte GetReceiverId() 
        {

            return ReceiverId; 

        }

        public byte[] GetMessageBytes() 
        { 

            return MessageBytes; 

        }
    }
}