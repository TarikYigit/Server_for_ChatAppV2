using Server_for_ChatApp.Interfaces;

namespace Server_for_ChatApp.Vault
{
    public class PermanentOfflineMessageStorage : IOfflineMessageStorage
    {

        private string myFile = @"C:\Users\Tarık\Desktop\Message_Save.txt";

        private string offlineMessageGroupFile = @"C:\Users\Tarık\Desktop\";


        public void AddNewMessageForUser(byte fromId, byte toId, byte[] data)
        {

            string base64Data = Convert.ToBase64String(data);

            string line = $"{fromId} {toId} {base64Data}";

            Console.WriteLine(line );

            File.AppendAllLines(myFile, new[] { line });

        }

        public List<byte[]> GetOfflineMessagesForUser(byte userId)
        {

            List<byte[]> userMessages = new List<byte[]>();

            if (!File.Exists(myFile)) return userMessages;

            string[] allLines = File.ReadAllLines(myFile);

            foreach (var line in allLines)
            {

                string[] parts = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length >= 3 && parts[1] == userId.ToString())
                {

                    try
                    {

                        byte senderId = byte.Parse(parts[0]);

                        byte receiverId = byte.Parse(parts[1]); 

                        byte[] textData = Convert.FromBase64String(parts[2]);

                        userMessages.Add(textData);
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }

            return userMessages;
        }

        public void ClearOfflineMessagesForUser(byte userId)
        {

            if (!File.Exists(myFile)) return;

            string[] allLines = File.ReadAllLines(myFile);

            List<string> linesToKeep = new List<string>();

            foreach (var line in allLines)
            {

                string[] parts = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length >= 3 && parts[1] == userId.ToString())
                {

                    continue;

                }

                linesToKeep.Add(line);
            }

            File.WriteAllLines(myFile, linesToKeep);
        }
        public void AddOfflineGroupMessage(byte targetUserId, byte[] payload)
        {
            string filePath = Path.Combine(offlineMessageGroupFile, $"{targetUserId}_grp_{System.DateTime.Now.Ticks}.msg");

            System.IO.File.WriteAllBytes(filePath, payload);
        }

        public List<byte[]> GetOfflineGroupMessagesForUser(byte userId)
        {
            List<byte[]> messages = new List<byte[]>();

            foreach (string file in Directory.GetFiles(offlineMessageGroupFile, $"{userId}_grp_*.msg"))
            {

                messages.Add(System.IO.File.ReadAllBytes(file));

            }
            return messages;
        }

        public void ClearOfflineGroupMessagesForUser(byte userId)
        {

            foreach (string file in Directory.GetFiles(offlineMessageGroupFile, $"{userId}_grp_*.msg"))
            {

                System.IO.File.Delete(file);

            }
        }
    }
}