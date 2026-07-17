using Server_for_ChatApp.Interfaces.RequestInterfaces;

namespace ServerForChatApp.Messages.ClientToServer
{
    public class LeaveGroupRequest : IRequest
    {
        public byte TargetUserId { get; private set; }
        public byte GroupId { get; private set; }

        public LeaveGroupRequest(byte[] payload)
        {
            if (payload.Length >= 2)
            {

                TargetUserId = payload[0];

                GroupId = payload[1];

            }
        }

        public byte GetId() => (byte)MessageId.LEAVE_GROUP;
    }
}