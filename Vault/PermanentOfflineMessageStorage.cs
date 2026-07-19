using Server_for_ChatApp.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Server_for_ChatApp.Vault
{
    public class PermanentOfflineMessageStorage : IOfflineMessageStorage
    {

        private string myFile = @"C:\Users\Tarık\Desktop\Message_Save.txt";

        private string offlineMessageGroupFile = @"C:\Users\Tarık\Desktop\";

        public void AddNewMessageForUser(byte fromId, byte toId, byte messageType, byte[] data)
        {

            string base64Data = Convert.ToBase64String(data);

            string line = $"{fromId} {toId} {messageType} {base64Data}";

            Console.WriteLine(line);

            File.AppendAllLines(myFile, new[] { line });

        }

        public List<Tuple<byte, byte[]>> GetOfflineMessagesForUser(byte userId)
        {

            List<Tuple<byte, byte[]>> userMessages = new List<Tuple<byte, byte[]>>();

            if (!File.Exists(myFile)) return userMessages;

            string[] allLines = File.ReadAllLines(myFile);

            foreach (var line in allLines)
            {

                string[] parts = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length >= 4 && parts[1] == userId.ToString())
                {

                    try
                    {

                        byte senderId = byte.Parse(parts[0]);

                        byte receiverId = byte.Parse(parts[1]);

                        byte messageType = byte.Parse(parts[2]); 

                        byte[] textData = Convert.FromBase64String(parts[3]);

                        userMessages.Add(new Tuple<byte, byte[]>(messageType, textData));

                    }
                    catch (Exception) { }
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

                if (parts.Length >= 4 && parts[1] == userId.ToString())
                {

                    continue;

                }
                linesToKeep.Add(line);

            }
            File.WriteAllLines(myFile, linesToKeep);
        }

        public void AddOfflineGroupMessage(byte targetUserId, byte messageType, byte[] payload)
        {

            string filePath = Path.Combine(offlineMessageGroupFile, $"{targetUserId}_grp_{messageType}_{System.DateTime.Now.Ticks}.msg");

            System.IO.File.WriteAllBytes(filePath, payload);

        }

        public List<Tuple<byte, byte[]>> GetOfflineGroupMessagesForUser(byte userId)
        {

            List<Tuple<byte, byte[]>> messages = new List<Tuple<byte, byte[]>>();

            string[] files = Directory.GetFiles(offlineMessageGroupFile, $"{userId}_grp_*.msg");

            var sortedFiles = files.OrderBy(file =>
            {

                string fileName = Path.GetFileNameWithoutExtension(file);

                string[] parts = fileName.Split('_');

                if (parts.Length >= 4 && long.TryParse(parts[3], out long ticks))
                {

                    return ticks;

                }
                return 0L; 

            }).ToList();

            foreach (string file in sortedFiles)
            {
                try
                {
                    string fileName = Path.GetFileNameWithoutExtension(file);
                    string[] parts = fileName.Split('_');
                    byte messageType = byte.Parse(parts[2]);

                    byte[] data = System.IO.File.ReadAllBytes(file);
                    messages.Add(new Tuple<byte, byte[]>(messageType, data));
                }
                catch { }
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