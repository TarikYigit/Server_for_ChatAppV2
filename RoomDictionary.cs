using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerForChatApp
{

    internal class RoomDictionary
    {

        public Dictionary<string, string> RoomDictionaryClient;

        public RoomDictionary() //  initialize the dictionary
        {

            RoomDictionaryClient = new Dictionary<string, string>();

        }



        public void AddItem(string key, string value)
        {

            if (!RoomDictionaryClient.ContainsKey(key)) 
            {

                RoomDictionaryClient.Add(key, value);

            }
        }



        // change log out data
        public void SetItem(string key, string value)
        {

            RoomDictionaryClient[key] = value;

        }



        //get log out data for message backlog
        public string GetItem(string key)
        {

            RoomDictionaryClient.TryGetValue(key, out string value);

            return value;

        }
    }
}
