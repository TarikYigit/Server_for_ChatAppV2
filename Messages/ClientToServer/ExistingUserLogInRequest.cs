using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server_for_ChatApp.Messages.ClientToServer
{
    internal class ExistingUserLogInRequest
    {
        private  string Username {  get; set; }

        public ExistingUserLogInRequest(byte[] payload)
        {

            Username = Encoding.UTF8.GetString(payload);

        }

        public string GetUsername()
        {

            return Username;

        }
    }
}
