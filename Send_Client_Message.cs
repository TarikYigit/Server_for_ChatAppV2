using System;
namespace Server_for_ChatApp
{
    public class Send_Client_Message
    {
        public static string file = @"C:\Users\tarik.dalkiran\Desktop\Workspace\Playground\Message_Save.txt";
        Room_Dictionary Room_Logs = new Room_Dictionary();

        public void Send_Client_Messages (string username)
        {   var UserDictionary = new UserDictionary();
            var Separate_Message_Dates = new Separate_Message_Dates();
            string[] user_logout_time = Separate_Message_Dates.Separate_Date_Info(UserDictionary.Get_Item(username));
            foreach (string line in System.IO.File.ReadLines(file))
            {
                string user_logout_time_string = "";
                string line_file_string = "";
                string[] file_line = Separate_Message_Dates.Separate_Date_Info(line);
                for (int i = 0; i<6; i++)   //getting the date and time from the log file and the user log out time to compare them
                {
                    user_logout_time_string += user_logout_time[i];
                    line_file_string += file_line[i];
                }
                if (Int32.Parse(user_logout_time_string) < Int32.Parse(line_file_string)) //format is YYYYMMDDHHMMSS so it is compared in integers
                {
                    string[] find_reciever = line.Split(" ");
                    if (find_reciever[2] == username && Room_Dictionary.Room_Logs)
                    {
                        //send message to client
                    }
                }
            }
        }
    }
}

