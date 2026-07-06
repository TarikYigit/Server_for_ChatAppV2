using System.IO;

namespace ServerForChatApp.Messages.ClientToServer
{
    internal class GetUserListRequest
    {
        public byte RequesterId { get; private set; }

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
    }
}