using ServerForChatApp;
using System.IO;
using System.Text;

namespace Server_for_ChatApp.Messages.ClientToServer
{

    internal class GetUserListForClient
    {

        private byte[] _userListPayload;

        public GetUserListForClient(RandomUserID idManager)
        {
            using (MemoryStream ms = new MemoryStream())

            using (BinaryWriter writer = new BinaryWriter(ms))
            {

                writer.Write((byte)idManager.UserIDDictionary.Count);

                foreach (var user in idManager.UserIDDictionary)
                {
                    writer.Write((byte)user.Key); 

                    byte[] usernameBytes = Encoding.UTF8.GetBytes(user.Value);

                    writer.Write((byte)usernameBytes.Length); 

                    writer.Write(usernameBytes); 

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