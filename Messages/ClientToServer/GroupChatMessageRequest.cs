using Server_for_ChatApp.Interfaces;
using Server_for_ChatApp.Interfaces.RequestInterfaces;
using ServerForChatApp;
using System.IO;
using System.Text;

namespace Server_for_ChatApp.Messages.ClientToServer
{
    internal class GroupChatMessageRequest : IRequest
    {
        public byte SenderId { get; private set; }
        public byte GroupId { get; private set; }

        public int messageid { get; private set; }

        public byte[] MessageBytes { get; private set; }

        public GroupChatMessageRequest(byte[] payload)
        {
            using (MemoryStream ms = new MemoryStream(payload))

            using (BinaryReader reader = new BinaryReader(ms))
            {

                SenderId = reader.ReadByte();

                GroupId = reader.ReadByte();

                messageid = reader.ReadInt32(); 

                int remainingBytes = (int)(ms.Length - ms.Position);

                if (remainingBytes > 0)
                {

                    MessageBytes = reader.ReadBytes(remainingBytes);

                }
            }
        }

        public byte GetId()
        {

            return (byte)MessageId.GROUP_CHAT_MESSAGE;

        }
    }
}