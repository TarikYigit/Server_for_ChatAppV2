using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ServerForChatApp.Messages.ClientToServer
{
    internal class GetUserListResponse : INetworkMessage
    {
        private byte[] _userListPayload;

        public GetUserListResponse(Dictionary<int, string> filteredUsers)
        {

            using (MemoryStream ms = new MemoryStream())

            using (BinaryWriter writer = new BinaryWriter(ms))

            {

                writer.Write((byte)filteredUsers.Count);

                foreach (var user in filteredUsers)
                {

                    writer.Write((byte)user.Key);

                    byte[] nameBytes = Encoding.UTF8.GetBytes(user.Value);

                    writer.Write((byte)nameBytes.Length);

                    writer.Write(nameBytes);

                }

                _userListPayload = ms.ToArray();

            }
        }

        public byte GetId()
        {

            return (byte)MessageId.GET_USERS;

        }

        public byte[] ToBytes()
        {

            return _userListPayload;

        }
    }
}