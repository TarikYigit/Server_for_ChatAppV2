using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server_for_ChatApp.UserManagers
{
    internal class UserInfo
    {
        public int ID { get; set; }

        public string username { get; set; }

        public string password { get; set; }

    }
}
