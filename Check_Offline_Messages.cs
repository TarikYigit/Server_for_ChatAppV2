using System;
using System.Collections.Generic;
using System.IO;

namespace Server_for_ChatApp
{
    internal static class Check_Offline_Messages
    {
        public static string file = @"C:\Users\tarik.dalkiran\Desktop\Workspace\Playground\Message_Save.txt";

        public static List<string> Get_And_Remove_Messages(int user_ID)
        {
            List<string> userMessages = new List<string>();
            List<string> linesToKeep = new List<string>();

            // Safety check: Don't crash if the file doesn't exist yet!
            if (!File.Exists(file)) return userMessages;

            foreach (var line in File.ReadLines(file))
            {
                string[] parts = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                // Make sure the line has enough parts before checking the ID
                if (parts.Length >= 4 && parts[2] == user_ID.ToString())
                {
                    // THE FIX: Hand the ENTIRE untouched line back to the server!
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