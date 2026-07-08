using Server_for_ChatApp.Interfaces.RequestInterfaces;
using System.IO;

namespace ServerForChatApp.Messages.ClientToServer
{
    internal class GetUserListRequest : IRequest
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

        byte GetUserID()
        {
            return RequesterId;
        }

    }
}