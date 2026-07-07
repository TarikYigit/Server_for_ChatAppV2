using Server_for_ChatApp.UserManagers;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServerForChatApp
{
    internal static class SendUserListForClient
    {
        // THE FIX: Accept the correct UserManagerClass
        public static byte[] GenerateUserListPacket(UserManager idManager)
        {
            List<byte> packet = new List<byte> { (byte)MessageId.GET_USERS };

            // 1. Get the list of users using your new Manager method!
            List<UserInfo> allUsers = idManager.GetAllUsers();

            packet.Add((byte)allUsers.Count);

            // 2. Loop through the UserInfo objects instead of a Dictionary
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