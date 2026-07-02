using ServerForChatApp;
using System.Collections.Generic;

namespace ServerForChatApp.Messages.ClientToServer
{
    internal class FetchOfflineMessagesForClient
    {
        public byte RequesterId { get; private set; }
        public List<string> OfflineMessages { get; private set; }

        public FetchOfflineMessagesForClient(byte[] payload)
        {
            RequesterId = payload[0];
            OfflineMessages = CheckOfflineMessages.GetAndRemoveMessages(RequesterId);
        }
    }
}