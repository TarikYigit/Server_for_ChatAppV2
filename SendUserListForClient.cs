using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerForChatApp
{
    internal static class SendUserListForClient
    {
        public static byte[] GenerateUserListPacket(RandomUserID idManager)
        {
            List<byte> packet = new List<byte> { (byte)MessageId.GET_USERS };            // user list message type

            packet.Add((byte)idManager.UserIDDictionary.Count);   //amount of usernames in the dictionary

            foreach (var user in idManager.UserIDDictionary)
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