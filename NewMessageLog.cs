using System;
using System.Collections.Generic;
using System.Text;

namespace ServerForChatApp
{
    internal class NewMessageLog
    {
        public static string file = @"C:\Users\tarik.dalkiran\Desktop\Workspace\Playground\Message_Save.txt"; //save all data here
        public static void AddNewMessage(string message)
        {
            System.IO.File.AppendAllText(file, message);
        }
    }
}
