using System;
using System.Collections.Generic;
using System.Text;

namespace Server_for_ChatApp
{
    internal class New_Message_Log
    {
        public static string file = @"C:\Users\tarik.dalkiran\Desktop\Workspace\Playground\Message_Save.txt"; //save all data here
        public static void Add_New_Message(string message)
        {
            System.IO.File.AppendAllText(file, message);
        }
    }
}
