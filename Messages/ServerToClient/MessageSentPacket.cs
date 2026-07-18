using Server_for_ChatApp.Messages.ServerInternals;
using ServerForChatApp;
using System.Text;

namespace Server_for_ChatApp.Messages.ServerToClient
{
    public class MessageSentPacket
    {
        private int _messageId;

        public MessageSentPacket(int messageId)
        {
            _messageId = messageId;
        }

        public byte GetId()
        {

            return (byte)MessageId.MESSAGE_SENT;

        }

        public byte[] ToBytes()
        {

            return BitConverter.GetBytes(_messageId);
        }
    }
}