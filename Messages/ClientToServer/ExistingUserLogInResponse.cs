using ServerForChatApp;
using ServerForChatApp.Messages;
using System.IO;
using System.Text;

namespace Server_for_ChatApp.Messages.ClientToServer
{
    internal class ExistingUserLogInResponse : INetworkMessage
    {

        public int LoggedInUserId { get; private set; }

        public bool IsValid { get; private set; }



        public ExistingUserLogInResponse(string username, Dictionary<int, string> userDictionary)
        {

            IsValid = false;

            foreach (var kvp in userDictionary)
            {

                if (kvp.Value == username)
                {

                    LoggedInUserId = kvp.Key;

                    IsValid = true;

                    break;

                }
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