using System.Collections.Generic;
using System.Net.Sockets;

namespace Server_for_ChatApp.ConnectionManagers
{
    public class ConnectionManager
    {
        private Dictionary<int, NetworkStream> _activeConnections;

        public ConnectionManager()
        {

            _activeConnections = new Dictionary<int, NetworkStream>();

        }

        public void AddConnection(int userId, NetworkStream stream)
        {

            if (_activeConnections.ContainsKey(userId))
            {

                _activeConnections[userId] = stream;

            }
            else
            {

                _activeConnections.Add(userId, stream);

            }
        }

        public void RemoveConnection(int userId)
        {

            if (_activeConnections.ContainsKey(userId))
            {

                _activeConnections.Remove(userId);

            }
        }

        public NetworkStream GetStream(int userId)
        {

            if (_activeConnections.TryGetValue(userId, out NetworkStream stream))
            {

                return stream;

            }
            return null;
        }

        public bool IsUserOnline(int userId)
        {

            return _activeConnections.ContainsKey(userId);

        }


    }
}