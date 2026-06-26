using System;
using System.Collections.Generic;
using System.Text;

namespace Server_for_ChatApp
{
    public class CheckUser
    {
        public List<string> Active_User_List;

        public void Set_User_Active(string username)
        {
            Active_User_List.Add(username);
        }

        public void Set_User_Offline(string username)
        {
            Active_User_List.Remove(username);
        }
    }
}
