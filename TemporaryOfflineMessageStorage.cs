using System;
using System.Collections.Generic;
using System.IO;

namespace ServerForChatApp
{
    public class TemporaryOfflineMessageStorage : IOfflineMessageStorage
    {
        private Dictionary<byte, List<byte[]>> inMemoryStorage = new Dictionary<byte, List<byte[]>>();

        public void AddNewMessageForUser(byte fromId, byte toId, byte[] data)
        {

            if (!inMemoryStorage.ContainsKey(toId))
            {

                inMemoryStorage[toId] = new List<byte[]>();

            }

            using (MemoryStream ms = new MemoryStream())

            using (BinaryWriter writer = new BinaryWriter(ms))
            {

                writer.Write(fromId); 

                writer.Write(data);   

                inMemoryStorage[toId].Add(ms.ToArray());

            }
        }

        public List<byte[]> GetOfflineMessagesForUser(byte userId)
        {

            if (inMemoryStorage.TryGetValue(userId, out List<byte[]> messages))
            {

                return new List<byte[]>(messages);

            }

            return new List<byte[]>(); // Return empty list if no messages exist
        }

        public void ClearOfflineMessagesForUser(byte userId)
        {

            if (inMemoryStorage.ContainsKey(userId))
            {

                inMemoryStorage.Remove(userId);

            }
        }
    }
}