using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server_for_ChatApp
{
    internal static class Check_Offline_Messages
    {
        public static string file = @"C:\Users\tarik.dalkiran\Desktop\Workspace\Playground\Message_Save.txt";
        public static List<string> Get_And_Remove_Messages(int user_ID)
        {
            List<string> userMessages = new List<string>();
            List<string> linesToKeep = new List<string>();
            foreach (var line in File.ReadLines(file))
            {
                string[] parts = line.Split(" ");

                if (parts[2] == user_ID.ToString())
                {
                    string message = line.Substring(line.IndexOf(parts[3]));
                    userMessages.Add(message);
                }
                else
                {
                    linesToKeep.Add(line);                  //Instead of last login I will just delete the sent messages from the message log
                }
            }

            File.WriteAllLines(file, linesToKeep);
            return userMessages;
        }
    }
}