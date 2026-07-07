using Server_for_ChatApp.UserManagers;
using ServerForChatApp;
using ServerForChatApp.Messages;
using System.IO;

namespace Server_for_ChatApp.Messages.ServerToClient
{
    internal class ExistingUserLogInResponse : INetworkMessage
    {
        public int LoggedInUserId { get; private set; }
        public bool IsValid { get; private set; }

        public ExistingUserLogInResponse(string username, UserManager usersManager, bool existInfo)
        {
            UserInfo user = usersManager.GetUserByName(username);

            if (existInfo)
            {

                LoggedInUserId = user.ID; 

                IsValid = true;

            }
            else
            {

                IsValid = false;

            }
        }

        public byte GetId()
        {

            return (byte)MessageId.LOG_IN;

        }

        public byte[] ToBytes()
        {

            using (MemoryStream ms = new MemoryStream())

            using (BinaryWriter writer = new BinaryWriter(ms))
            {

                if (IsValid)
                {

                    writer.Write((byte)0x01); // 0x01 = Accepted

                    writer.Write((byte)LoggedInUserId);

                }
                else
                {

                    writer.Write((byte)0x02); // 0x02 = Rejected

                }

                return ms.ToArray();
            }
        }
    }
}