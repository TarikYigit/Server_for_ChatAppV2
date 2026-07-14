using Server_for_ChatApp.Managers.UserManagers;
using System.Text;

namespace ServerForChatApp
{
    internal static class SendUserListForClient
    {
        public static byte[] GenerateUserListPacket(UserManager idManager)
        {

            List<byte> packet = new List<byte> { (byte)MessageId.GET_USERS };

            List<UserInfo> allUsers = idManager.GetAllUsers();

            packet.Add((byte)allUsers.Count);

            foreach (UserInfo user in allUsers)
            {

                byte userId = (byte)user.ID; // Change to user.ID if your property is capitalized

                byte[] usernameBytes = Encoding.UTF8.GetBytes(user.username);

                int length = usernameBytes.Length;

                if (length > 255) length = 255;

                packet.Add(userId);

                packet.Add((byte)length);

                packet.AddRange(usernameBytes);

            }

            return packet.ToArray();
        }
    }
}