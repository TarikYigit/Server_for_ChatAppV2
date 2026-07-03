using System;
using System.Collections.Generic;
using System.Text;

namespace ServerForChatApp
{

    public class UserDictionary
    {

        public Dictionary<string, string> UserLogData;

        public UserDictionary() //  initialize the dictionary
        {

            UserLogData = new Dictionary<string, string>();

        }

        // add new user
        public void AddItem(string key, string value)
        {

            if (!UserLogData.ContainsKey(key)) //since I check for existing users, the code in program.cs will not overwrite existing users log out time
            {

                UserLogData.Add(key, value);

            }
        }

        // change log out data
        public void SetItem(string key, string value)
        {

            UserLogData[key] = value;

        }

        //get log out data for message backlog
        public string GetItem(string key)
        {

            UserLogData.TryGetValue(key, out string value);

            return value;
           
        }
    }
}