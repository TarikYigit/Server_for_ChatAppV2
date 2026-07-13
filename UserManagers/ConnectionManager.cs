using Server_for_ChatApp.Interfaces;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Server_for_ChatApp.ConnectionManagers
{

    public class ConnectionManager : IConnections
    {
        public Dictionary<int, NetworkStream> _activeConnections;

        public ConnectionManager()
        {

            _activeConnections = new Dictionary<int, NetworkStream>();

        }

        public List<int> GetAllOnline()
        {

            List<int> UserIDs = new List<int>();

            foreach (var connection in _activeConnections.Keys)
                UserIDs.Add(connection);

            return UserIDs;

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

        public static void Send(byte messageId, byte[] payload, NetworkStream stream)
        {
            if (stream != null)
            {

                WriteToStream(stream, messageId, payload);

            }
        }


        public static void Send(NetworkStream stream, byte messageId, byte[] payload)
        {

            if (stream != null)
            {

                WriteToStream(stream, messageId, payload);

            }
        }

        public static void Send(NetworkStream stream, INetworkMessage message)
        {

            Send(stream, message.GetId(), message.ToBytes());

        }

        private static void WriteToStream(NetworkStream myStream, byte messageId, byte[] payload)
        {

            int payloadLength = payload?.Length ?? 0;

            using (MemoryStream ms = new MemoryStream())

            using (BinaryWriter writer = new BinaryWriter(ms))
            {

                writer.Write(messageId);

                writer.Write(payloadLength);

                if (payloadLength > 0)
                {

                    writer.Write(payload);

                }

                byte[] finalPacket = ms.ToArray();

                myStream.Write(finalPacket, 0, finalPacket.Length);

                myStream.Flush();

            }
        }


    }
}