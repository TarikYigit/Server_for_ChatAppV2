using System;
using System.Collections.Generic;

namespace Server_for_ChatApp
{
    internal class Random_User_ID
    {
        public Dictionary<int, string> User_ID_Dictionary;
        private Random _random;

        public Random_User_ID() 
        {
            User_ID_Dictionary = new Dictionary<int, string>();
            _random = new Random();
        }

        // add new user
        public void Add_Item(int key, string value)
        {
            if (!User_ID_Dictionary.ContainsKey(key))
            {
                User_ID_Dictionary.Add(key, value);
            }
        }

        public string Get_Item(int key)
        {
            User_ID_Dictionary.TryGetValue(key, out string value);
            return value;
        }

        public int Generate_Random_User_ID(string username)
        {
            while (true)
            {
                int randomNumber = _random.Next(1, 255);

                if (!User_ID_Dictionary.ContainsKey(randomNumber))
                {
                    User_ID_Dictionary.Add(randomNumber, username);
                    return randomNumber; // Return the generated ID back to the server
                }
            }
        }
    }
}