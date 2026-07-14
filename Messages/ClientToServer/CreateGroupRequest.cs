using ServerForChatApp;
using Server_for_ChatApp.Interfaces.RequestInterfaces;
using System.Text;

namespace Server_for_ChatApp.Messages.ClientToServer
{
    internal class CreateGroupRequest : IRequest 
    {
        public string GroupName { get; private set; }
        public List<int> UserIdsToInvite { get; private set; } 

        public CreateGroupRequest(byte[] payload)
        {

            UserIdsToInvite = new List<int>();

            using (MemoryStream ms = new MemoryStream(payload))

            using (BinaryReader reader = new BinaryReader(ms))
            {

                byte nameLength = reader.ReadByte();

                byte[] nameBytes = reader.ReadBytes(nameLength);

                GroupName = Encoding.UTF8.GetString(nameBytes);

                byte userCount = reader.ReadByte();

                for (int i = 0; i < userCount; i++)
                {

                    UserIdsToInvite.Add((int)reader.ReadByte());

                }
            }
        }

        public byte GetId()
        {

            return (byte)MessageId.CREATE_GROUP;

        }
    }
}