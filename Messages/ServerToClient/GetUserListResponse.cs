using Server_for_ChatApp.Interfaces;
using Server_for_ChatApp.UserManagers;
using ServerForChatApp;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Server_for_ChatApp.Messages.ServerToClient
{
    internal class GetUserListResponse : INetworkMessage
    {
        private List<UserInfo> _userList;

        public GetUserListResponse(List<UserInfo> userList)
        {

            _userList = userList;

        }

        public byte GetId()
        {

            return (byte)MessageId.GET_USERS;

        }

        public byte[] ToBytes()
        {
            using (MemoryStream ms = new MemoryStream())
            using (BinaryWriter writer = new BinaryWriter(ms))
            {
                writer.Write((byte)_userList.Count);

                foreach (UserInfo user in _userList)
                {
                    writer.Write((byte)user.ID);

                    byte[] nameBytes = Encoding.UTF8.GetBytes(user.username);
                    writer.Write((byte)nameBytes.Length);
                    writer.Write(nameBytes);
                }

                return ms.ToArray();
            }
        }
    }
}