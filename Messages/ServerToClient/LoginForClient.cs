using Server_for_ChatApp.Messages.ClientToServer;
using ServerForChatApp;
using ServerForChatApp.Messages;
using System.IO;
using System.Text;

namespace Server_for_ChatApp.Messages.ServerToClient
{
    internal class LoginForClient : INetworkMessage
    {

        public string Username { get; private set; }

        public int AssignedId { get; private set; }

        public bool IsAccepted { get; private set; }

        public LoginForClient(byte[] payload, UserManagerClass idManager, UserDictionary userLogs, bool isAccepted)
        {

            if (isAccepted)
            {
                Username = Encoding.UTF8.GetString(payload);

                AssignedId = idManager.GenerateRandomUserID(Username);

                userLogs.AddItem(Username, "0000-00-00-00:00:00");

            }

            IsAccepted = isAccepted;


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

                if (IsAccepted)
                {

                    writer.Write((byte)0x01); // 0x01 = Accepted

                    writer.Write((byte)AssignedId);

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