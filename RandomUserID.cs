using System;
using System.Collections.Generic;

namespace ServerForChatApp
{
    internal class RandomUserID
    {
        public Dictionary<int, string> UserIDDictionary;

        private Random _random;



        public RandomUserID() 
        {
            UserIDDictionary = new Dictionary<int, string>();
            _random = new Random();
        }


        // add new user
        public void AddItem(int key, string value)
        {

            if (!UserIDDictionary.ContainsKey(key))
            {

                UserIDDictionary.Add(key, value);

            }
        }

        public string GetItem(int key)
        {

            UserIDDictionary.TryGetValue(key, out string value);

            return value;

        }

        public int GenerateRandomUserID(string username)
        {

            while (true)
            {

                int randomNumber = _random.Next(1, 255);

                if (!UserIDDictionary.ContainsKey(randomNumber))
                {

                    UserIDDictionary.Add(randomNumber, username);

                    return randomNumber; // Return the generated ID back to the server

                }
            }
        }
    }
}