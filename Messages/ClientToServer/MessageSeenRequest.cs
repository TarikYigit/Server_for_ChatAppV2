using System.IO;
using Server_for_ChatApp.Interfaces.RequestInterfaces;
using ServerForChatApp;

namespace Server_for_ChatApp.Messages.ClientToServer
{
    public class MessageSeenRequest : IRequest
    {
        public byte OriginalSenderId { get; private set; }
        public int SeenMessageId { get; private set; }

        public MessageSeenRequest(byte[] payload)
        {

            using (MemoryStream ms = new MemoryStream(payload))

            using (BinaryReader reader = new BinaryReader(ms))
            {

                OriginalSenderId = reader.ReadByte();

                SeenMessageId = reader.ReadInt32();

            }
        }

        public byte GetId()
        {

            return (byte)MessageId.MESSAGE_SEEN;

        }
    }
}