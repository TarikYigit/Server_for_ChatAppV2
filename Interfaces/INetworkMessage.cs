namespace Server_for_ChatApp.Interfaces
{
    public interface INetworkMessage
    {
        byte GetId();
        byte[] ToBytes();
    }
}