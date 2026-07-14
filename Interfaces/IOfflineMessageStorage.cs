namespace Server_for_ChatApp.Interfaces
{
    public interface IOfflineMessageStorage
    {
        void AddNewMessageForUser(byte fromId, byte toId, byte[] data);

        List<byte[]> GetOfflineMessagesForUser(byte userId);

        void ClearOfflineMessagesForUser(byte userId);

        void AddOfflineGroupMessage(byte targetUserId, byte[] payload);
        List<byte[]> GetOfflineGroupMessagesForUser(byte targetUserId);
        void ClearOfflineGroupMessagesForUser(byte targetUserId);
    }
}