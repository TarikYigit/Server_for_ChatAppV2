//using Server_for_ChatApp.Interfaces;

//namespace Server_for_ChatApp.Vault
//{
//    public class TemporaryOfflineMessageStorage : IOfflineMessageStorage
//    {
//        private Dictionary<byte, List<byte[]>> inMemoryStorage = new Dictionary<byte, List<byte[]>>();

//        public void AddNewMessageForUser(byte fromId, byte toId, byte[] data)
//        {

//            if (!inMemoryStorage.ContainsKey(toId))
//            {

//                inMemoryStorage[toId] = new List<byte[]>();

//            }

//            using (MemoryStream ms = new MemoryStream())

//            using (BinaryWriter writer = new BinaryWriter(ms))
//            {

//                writer.Write(fromId); 

//                writer.Write(data);   

//                inMemoryStorage[toId].Add(ms.ToArray());

//            }
//        }

//        public List<byte[]> GetOfflineMessagesForUser(byte userId)
//        {

//            if (inMemoryStorage.TryGetValue(userId, out List<byte[]> messages))
//            {

//                return new List<byte[]>(messages);

//            }

//            return new List<byte[]>(); // Return empty list if no messages exist
//        }

//        public void ClearOfflineMessagesForUser(byte userId)
//        {

//            if (inMemoryStorage.ContainsKey(userId))
//            {

//                inMemoryStorage.Remove(userId);

//            }
//        }

//        public void AddOfflineGroupMessage(byte targetUserId, byte[] payload)
//        {
//            string filePath = Path.Combine(offlineMessageGroupFile, $"{targetUserId}_grp_{System.DateTime.Now.Ticks}.msg");

//            System.IO.File.WriteAllBytes(filePath, payload);
//        }

//        public List<byte[]> GetOfflineGroupMessagesForUser(byte userId)
//        {
//            List<byte[]> messages = new List<byte[]>();

//            foreach (string file in Directory.GetFiles(offlineMessageGroupFile, $"{userId}_grp_*.msg"))
//            {

//                messages.Add(System.IO.File.ReadAllBytes(file));

//            }
//            return messages;
//        }

//        public void ClearOfflineGroupMessagesForUser(byte userId)
//        {

//            foreach (string file in Directory.GetFiles(offlineMessageGroupFile, $"{userId}_grp_*.msg"))
//            {

//                System.IO.File.Delete(file);

//            }
//        }
//    }
//}