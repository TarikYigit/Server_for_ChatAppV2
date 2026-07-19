using Server_for_ChatApp.Interfaces;
using ServerForChatApp;
using System.IO;

namespace Server_for_ChatApp.Messages.ServerToClient
{
    public class GroupImageMessageResponse : INetworkMessage
    {
        public byte SenderId { get; private set; }

        public byte GroupId { get; private set; }

        public int Messageid { get; private set; }

        public byte[] ImageBytes { get; private set; }

        public GroupImageMessageResponse(byte senderId, byte groupId, int messageId, byte[] imageBytes)
        {

            SenderId = senderId;

            GroupId = groupId;

            Messageid = messageId;

            ImageBytes = imageBytes;

        }

        public byte GetId() => (byte)MessageId.GROUP_IMAGE;

        public byte[] ToBytes()
        {

            using (MemoryStream ms = new MemoryStream())

            using (BinaryWriter writer = new BinaryWriter(ms))
            {

                writer.Write(SenderId);

                writer.Write(GroupId);

                writer.Write(Messageid);

                writer.Write(ImageBytes);

                return ms.ToArray();

            }
        }
    }
}