using Server_for_ChatApp.Interfaces.RequestInterfaces;
using ServerForChatApp;


namespace Server_for_ChatApp.Messages.ClientToServer
{
    public class GroupImageMessageRequest : IRequest
    {

        public byte SenderId { get; private set; }

        public byte GroupId { get; private set; }

        public int Messageid { get; private set; }

        public byte[] ImageBytes { get; private set; }

        public GroupImageMessageRequest(byte[] payload)
        {

            using (MemoryStream ms = new MemoryStream(payload))

            using (BinaryReader reader = new BinaryReader(ms))
            {

                SenderId = reader.ReadByte();

                GroupId = reader.ReadByte();

                Messageid = reader.ReadInt32();

                ImageBytes = reader.ReadBytes((int)(ms.Length - ms.Position));

            }
        }

        public byte GetId()
        {

            return (byte)MessageId.GROUP_IMAGE;

        }
    }
}