namespace ServerForChatApp.Messages
{
    public interface INetworkMessage
    {
        byte GetId();
        byte[] ToBytes();
    }
}