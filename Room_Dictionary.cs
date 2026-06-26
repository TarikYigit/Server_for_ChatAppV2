using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server_for_ChatApp
{
    internal class Room_Dictionary
    {
        public Dictionary<string, string> Room_Dictionary_Client;

        public Room_Dictionary() //  initialize the dictionary
        {
            Room_Dictionary_Client = new Dictionary<string, string>();
        }
        public void Add_Item(string key, string value)
        {
            if (!Room_Dictionary_Client.ContainsKey(key)) 
            {
                Room_Dictionary_Client.Add(key, value);
            }
        }

        // change log out data
        public void Set_Item(string key, string value)
        {
            Room_Dictionary_Client[key] = value;
        }

        //get log out data for message backlog
        public string Get_Item(string key)
        {
            Room_Dictionary_Client.TryGetValue(key, out string value);
            return value;

        }
    }
}
