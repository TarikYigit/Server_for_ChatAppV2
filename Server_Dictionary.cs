using System;
using System.Collections.Generic;
using System.Text;

namespace Server_for_ChatApp
{

    public class UserDictionary
    {
        public Dictionary<string, string> User_Log_Data;

        public UserDictionary() //  initialize the dictionary
        {
            User_Log_Data = new Dictionary<string, string>();
        }

        // add new user
        public void AddItem(string key, string value)
        {
            if (!User_Log_Data.ContainsKey(key)) //since I check for existing users, the code in program.cs will not overwrite existing users log out time
            {
                User_Log_Data.Add(key, value);
            }
        }

        // change log out data
        public void SetItem(string key, string value)
        {
            User_Log_Data[key] = value;
        }

        //get log out data for message backlog
        public string Get_Item(string key)
        {
            User_Log_Data.TryGetValue(key, out string value);
            return value;
           
        }
    }
}