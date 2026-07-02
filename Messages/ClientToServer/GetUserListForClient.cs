using ServerForChatApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerForChatApp.Messages.ClientToServer
{
    internal class GetUserListForClient
    {   
        public byte RequesterId { get; private set; }
        public byte[] UserListPayload { get; private set; }

        public GetUserListForClient(byte[] payload, RandomUserID idManager)
        {
            if (payload != null && payload.Length > 0)
            {
                RequesterId = payload[0];
            }

            List<byte> packetList = new List<byte>();

            packetList.Add((byte)idManager.UserIDDictionary.Count);

            foreach (var user in idManager.UserIDDictionary)
            {
                byte userId = (byte)user.Key;
                byte[] usernameBytes = Encoding.UTF8.GetBytes(user.Value);

                int length = usernameBytes.Length;
                if (length > 255) length = 255;

                packetList.Add(userId);
                packetList.Add((byte)length);

                for (int i = 0; i < length; i++)
                {
                    packetList.Add(usernameBytes[i]);
                }
            }

            UserListPayload = packetList.ToArray();
        }
    }
}
