using Server_for_ChatApp.Interfaces;
using Server_for_ChatApp.Interfaces.RequestInterfaces;
using ServerForChatApp;
using System.IO;

namespace Server_for_ChatApp.Messages.ClientToServer
{
    public class ImageMessageRequest : IRequest
    {
        public byte SenderId { get; private set; }
        public byte ReceiverId { get; private set; }
        public int Messageid { get; private set; }
        public byte[] ImageBytes { get; private set; }

        public ImageMessageRequest(byte[] payload)
        {

            using (MemoryStream ms = new MemoryStream(payload))

            using (BinaryReader reader = new BinaryReader(ms))
            {

                SenderId = reader.ReadByte();

                ReceiverId = reader.ReadByte();

                Messageid = reader.ReadInt32();

                ImageBytes = reader.ReadBytes((int)(ms.Length - ms.Position));

            }
        }

        public byte GetId() => (byte)MessageId.SEND_IMAGE;
    }
}