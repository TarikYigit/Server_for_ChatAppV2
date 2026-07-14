using Server_for_ChatApp.Interfaces.RequestInterfaces;
using ServerForChatApp;

namespace Server_for_ChatApp.Messages.ClientToServer
{
    internal class LogOutRequest : IRequest
    {
        public byte UserId { get; private set; }
        public LogOutRequest(byte ID)
        {

            UserId = ID;

        }
        public byte GetId()
        {

            return (byte)MessageId.LOG_OUT;

        }

        public byte GetUserID()
        {
            return (byte)UserId;
        }

    }
}
