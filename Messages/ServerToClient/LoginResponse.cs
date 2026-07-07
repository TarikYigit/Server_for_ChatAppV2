using Server_for_ChatApp.Messages.ClientToServer;
using Server_for_ChatApp.UserManagers;
using ServerForChatApp;
using ServerForChatApp.Messages;
using System.IO;
using System.Text;

namespace Server_for_ChatApp.Messages.ServerToClient
{
    internal class LoginResponse : INetworkMessage
    {

        public bool IsAccepted { get; set; }

        public int AssignedId { get; private set; }

        public LoginResponse(int assignedID, bool isAccepted)
        {

            AssignedId = assignedID;

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