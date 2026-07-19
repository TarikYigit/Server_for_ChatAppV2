namespace Server_for_ChatApp.Interfaces
{
    public interface IOfflineMessageStorage
    {

        void AddNewMessageForUser(byte fromId, byte toId, byte messageType, byte[] data);

        List<Tuple<byte, byte[]>> GetOfflineMessagesForUser(byte userId);

        void ClearOfflineMessagesForUser(byte userId);

        void AddOfflineGroupMessage(byte targetUserId, byte messageType, byte[] payload);

        List<Tuple<byte, byte[]>> GetOfflineGroupMessagesForUser(byte userId);

        void ClearOfflineGroupMessagesForUser(byte userId);

    }
}   