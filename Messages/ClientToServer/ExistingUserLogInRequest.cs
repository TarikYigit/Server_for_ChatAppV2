using ServerForChatApp;
using System.IO;
using System.Text;

namespace Server_for_ChatApp.Messages.ClientToServer
{
    internal class ExistingUserLogInRequest
    {
        public string Username { get; private set; }

        public int LoggedInUserId { get; private set; }

        public bool IsValid { get; private set; }



        public ExistingUserLogInRequest(byte[] payload, RandomUserID idManager)
        {

            Username = Encoding.UTF8.GetString(payload);

            IsValid = false;

            foreach (var kvp in idManager.UserIDDictionary)
            {

                if (kvp.Value == Username)
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