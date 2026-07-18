using Server_for_ChatApp.Interfaces;
using ServerForChatApp;
using System;
using System.IO;

namespace Server_for_ChatApp.Messages.ServerToClient
{
    public class GroupMessageResponse : INetworkMessage
    {

        private byte _senderId;

        private byte _groupId;

        private byte[] _messageBytes;

        private long _serverTimestamp;

        private int _messageId;

        public GroupMessageResponse(byte senderId, byte groupId, int messageid, byte[] messageBytes)
        {

            _senderId = senderId;

            _groupId = groupId;

            _messageId = messageid;

            _messageBytes = messageBytes;

            _serverTimestamp = DateTime.Now.Ticks;

        }

        public byte GetId() => (byte)MessageId.GROUP_CHAT_MESSAGE;

        public byte[] ToBytes()
        {

            using (MemoryStream ms = new MemoryStream())

            using (BinaryWriter writer = new BinaryWriter(ms))
            {

                writer.Write(_senderId);

                writer.Write(_groupId);

                writer.Write(_messageId); 

                writer.Write(_serverTimestamp);

                writer.Write(_messageBytes);

                return ms.ToArray();

            }
        }
    }
}