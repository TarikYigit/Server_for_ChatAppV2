using Server_for_ChatApp.ConnectionManagers;
using Server_for_ChatApp.Interfaces;
using System.IO;
using System.Net.Sockets;

namespace Server_for_ChatApp
{
    internal static class SendPacketClass
    {
        public static void Send(int userID, byte messageId, byte[] payload, ConnectionManager serverConnections)
        {

            NetworkStream stream = serverConnections.GetStream(userID);

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