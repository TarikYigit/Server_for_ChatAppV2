using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server_for_ChatApp
{
    internal static class Send_User_List
    {
        public static byte[] GenerateUserListPacket(Random_User_ID idManager)
        {
            List<byte> packet = new List<byte> { 0x02 };            // user list message type

            packet.Add((byte)idManager.User_ID_Dictionary.Count);   //amount of usernames in the dictionary

            foreach (var user in idManager.User_ID_Dictionary)
            {
                byte userId = (byte)user.Key;
                byte[] usernameBytes = Encoding.UTF8.GetBytes(user.Value);

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