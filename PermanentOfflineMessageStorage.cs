using System;
using System.Collections.Generic;
using System.IO;

namespace ServerForChatApp
{
    public class PermanentOfflineMessageStorage : IOfflineMessageStorage
    {

        private string myFile = @"C:\Users\tarik.dalkiran\Desktop\Workspace\Playground\Message_Save.txt";

        public void AddNewMessageForUser(byte fromId, byte toId, byte[] data)
        {

            string base64Data = Convert.ToBase64String(data);

            string line = $"{fromId} {toId} {base64Data}";

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

                        using (MemoryStream ms = new MemoryStream())

                        using (BinaryWriter writer = new BinaryWriter(ms))
                        {

                            writer.Write(senderId);

                            writer.Write(textData); 

                            userMessages.Add(ms.ToArray());

                        }
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
    }
}