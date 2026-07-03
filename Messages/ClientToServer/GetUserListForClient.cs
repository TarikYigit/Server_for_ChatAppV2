using ServerForChatApp;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server_for_ChatApp.Messages.ClientToServer
{
    internal class GetUserListForClient
    {
        public byte RequesterId { get; private set; }
        private byte[] _userListPayload;

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

            _userListPayload = packetList.ToArray();
        }

        public byte GetId()
        {
            return (byte)MessageId.GET_USERS;
        }

        public byte[] ToBytes()
        {
            return _userListPayload;
        }
    }
}