using Server_for_ChatApp.Interfaces;
using ServerForChatApp;

namespace Server_for_ChatApp.Messages.ServerToClient
{
    internal class RegisterResponse : INetworkMessage
    {

        public bool IsAccepted { get; set; }

        public bool PasswordStrong { get; set; }
        public int AssignedId { get; private set; }

        public RegisterResponse(int assignedID, bool isAccepted, bool passwordStrong)
        {

            AssignedId = assignedID;

            IsAccepted = isAccepted;

            PasswordStrong = passwordStrong;

        }


        public byte GetId()
        {

            return (byte)MessageId.REGISTER;

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

                    if (!PasswordStrong)
                    {
                        writer.Write((byte)0x01); // Reason 0x01 = Password Weak
                    }
                    else
                    {
                        writer.Write((byte)0x02); // Reason 0x02 = Username Taken
                    }
                }
                return ms.ToArray();
            }
        }
    }
}