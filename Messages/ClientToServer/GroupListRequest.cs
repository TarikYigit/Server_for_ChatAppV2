using Server_for_ChatApp.Interfaces.RequestInterfaces;

namespace ServerForChatApp.Messages.ClientToServer
{
    public class GroupListRequest : IRequest
    {
        public byte RequestedUserId { get; private set; }

        public GroupListRequest(byte[] payload)
        {
            if (payload != null && payload.Length >= 1)
            {

                RequestedUserId = payload[0];

            }
        }

        public byte GetId() => (byte)MessageId.GROUP_LIST;
    }
}