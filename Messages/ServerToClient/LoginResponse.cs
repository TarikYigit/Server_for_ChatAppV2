using Server_for_ChatApp.Interfaces;
using ServerForChatApp;

namespace Server_for_ChatApp.Messages.ServerToClient
{
    internal class LoginResponse : INetworkMessage
    {
        public int LoggedInUserId { get; private set; }

        public bool IsValid { get; private set; }

        public LoginResponse(string username, int UserID, bool existInfo)
        {

            LoggedInUserId = UserID;

            IsValid = existInfo;

        }

        public byte GetId()
        {

            return (byte)MessageId.LOGIN;

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