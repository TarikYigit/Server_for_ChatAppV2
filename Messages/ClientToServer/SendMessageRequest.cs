using Server_for_ChatApp.Interfaces.RequestInterfaces;
using ServerForChatApp;

namespace Server_for_ChatApp.Messages.ClientToServer
{
    internal class SendMessageRequest : IRequest
    {
        private byte SenderId { get; set; }
        private byte ReceiverId { get; set; }
        private byte[] MessageBytes { get; set; }

        private int _messageId; 
        private long TimeStamp { get; set; }

        public SendMessageRequest(byte[] payload)
        {
            using (MemoryStream ms = new MemoryStream(payload))

            using (BinaryReader reader = new BinaryReader(ms))
            {

                SenderId = reader.ReadByte();

                ReceiverId = reader.ReadByte();

                _messageId = reader.ReadInt32(); 

                int remainingBytes = (int)(ms.Length - ms.Position);

                MessageBytes = reader.ReadBytes(remainingBytes);

                TimeStamp = DateTime.Now.Ticks;

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

        public byte GetId()
        {

            return (byte)MessageId.SEND_MESSAGE;

        }

        public int GetMessageId()
        {
            return _messageId;
        }

        public long GetTimeStamp()
        {

            return TimeStamp;

        }
    }
}