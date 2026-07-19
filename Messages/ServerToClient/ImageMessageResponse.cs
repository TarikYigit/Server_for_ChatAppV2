using Server_for_ChatApp.Interfaces;
using ServerForChatApp;
using System.IO;

namespace Server_for_ChatApp.Messages.ServerToClient
{
    public class ImageMessageResponse : INetworkMessage
    {
        public byte SenderId { get; private set; }

        public int Messageid { get; private set; }

        public byte[] ImageBytes { get; private set; }

        public ImageMessageResponse(byte senderId, int messageId, byte[] imageBytes)
        {

            SenderId = senderId;

            Messageid = messageId;

            ImageBytes = imageBytes;

        }

        public byte GetId() => (byte)MessageId.SEND_IMAGE;

        public byte[] ToBytes()
        {

            using (MemoryStream ms = new MemoryStream())

            using (BinaryWriter writer = new BinaryWriter(ms))
            {

                writer.Write(SenderId);

                writer.Write(Messageid);

                writer.Write(ImageBytes);

                return ms.ToArray();
            }
        }
    }
}