using System;
using System.Collections.Generic;
using System.IO;

namespace ServerForChatApp
{
    internal static class CheckOfflineMessages
    {
        public static string file = @"C:\Users\tarik.dalkiran\Desktop\Workspace\Playground\Message_Save.txt";

        public static List<string> GetAndRemoveMessages(int user_ID)
        {
            List<string> userMessages = new List<string>();
            List<string> linesToKeep = new List<string>();

            if (!File.Exists(file)) return userMessages;

            foreach (var line in File.ReadLines(file))
            {
                string[] parts = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length >= 4 && parts[2] == user_ID.ToString())
                {
                    userMessages.Add(line);
                }
                else
                {
                    linesToKeep.Add(line);
                }
            }

            File.WriteAllLines(file, linesToKeep);
            return userMessages;
        }
    }
}