using Server_for_ChatApp.Interfaces;
using Server_for_ChatApp.Managers.GroupChatManager;
using ServerForChatApp;

using System.Text;

namespace Server_for_ChatApp.Messages.ServerToClient
{
    public class GroupListGet : INetworkMessage
    {
        private List<GroupChatInfo> _groups;

        public GroupListGet(List<GroupChatInfo> groups)
        {

            _groups = groups;

        }

        public byte GetId() => (byte)MessageId.GROUP_LIST;

        public byte[] ToBytes()
        {
            using (MemoryStream ms = new MemoryStream())

            using (BinaryWriter writer = new BinaryWriter(ms))
            {
                writer.Write((byte)_groups.Count);

                foreach (var group in _groups)
                {

                    writer.Write((byte)group.GroupChatID);

                    byte[] nameBytes = Encoding.UTF8.GetBytes(group.GroupName);

                    writer.Write((byte)nameBytes.Length);

                    writer.Write(nameBytes);

                }
                return ms.ToArray();
            }
        }
    }
}