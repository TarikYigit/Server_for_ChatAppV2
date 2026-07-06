using System.Collections.Generic;

namespace ServerForChatApp
{
    public interface IOfflineMessageStorage
    {
        void AddNewMessageForUser(byte fromId, byte toId, byte[] data);

        List<byte[]> GetOfflineMessagesForUser(byte userId);

        void ClearOfflineMessagesForUser(byte userId);

    }
}