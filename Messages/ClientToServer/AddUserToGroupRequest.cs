using Server_for_ChatApp.Interfaces.RequestInterfaces;

namespace ServerForChatApp.Messages.ClientToServer
{
    public class AddUserToGroupRequest : IRequest
    {
        public byte GroupId { get; private set; }
        public byte UserToAddId { get; private set; }

        public AddUserToGroupRequest(byte[] payload)
        {

            GroupId = payload[0];

            UserToAddId = payload[1];

        }

        public byte GetId() => (byte)MessageId.ADD_USER_TO_GROUP;

    }
}