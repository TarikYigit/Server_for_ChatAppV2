using Server_for_ChatApp.Interfaces.RequestInterfaces;
using System.IO;

namespace ServerForChatApp.Messages.ClientToServer
{
    public class FetchOfflineMessageRequest : IRequest
    {
        public byte RequesterId { get; private set; }

        public FetchOfflineMessageRequest(byte[] payload)
        {

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
            return (byte)MessageId.FETCH_OFFLINE_MESSAGES;
        }

    }
}