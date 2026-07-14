using Server_for_ChatApp.Interfaces.RequestInterfaces;

namespace ServerForChatApp.Messages.ClientToServer
{
    public class GetUserListRequest : IRequest
    {
        private byte RequesterId { get; set; }

        public GetUserListRequest(byte[] payload)
        {
            // request = GetUserRequest(userId)
            if (payload != null && payload.Length > 0)
            {

                using (MemoryStream ms = new MemoryStream(payload))

                using (BinaryReader reader = new BinaryReader(ms))
                {

                    RequesterId = reader.ReadByte();

                }
            }
        }

        public byte GetUserID()
        {
            return RequesterId;
        }
        
        public byte GetId()
        {
            return (byte)MessageId.GET_USERS;
        }
    }
}