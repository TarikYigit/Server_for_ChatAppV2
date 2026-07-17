using ServerForChatApp;
using System.Text;

namespace Server_for_ChatApp.Messages.ServerToClient
{
    public class MessageSeenPacket
    {
        private int _messageId;

        public MessageSeenPacket(int messageId)
        {
            _messageId = messageId;
        }

        public byte GetId()
        {

            return (byte)MessageId.MESSAGE_SEEN;

        }

        public byte[] ToBytes()
        {

            return BitConverter.GetBytes(_messageId);

        }
    }
}