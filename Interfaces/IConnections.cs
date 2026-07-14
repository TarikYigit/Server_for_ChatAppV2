using System.Net.Sockets;

namespace Server_for_ChatApp.Interfaces
{
    public interface IConnections
    {
        void AddConnection(int userId, NetworkStream stream);

        void RemoveConnection(int userId);

        bool IsUserOnline(int userId);

        NetworkStream GetStream(int userId);


    }
}