using ServerForChatApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server_for_ChatApp.Messages.ClientToServer
{
    internal class LoginRequest
    {   public bool IsAccepted {  get; set; }

        public string Username { get; private set; }
        public LoginRequest(byte[] payload, List<string> usernameListCopy)
        {

            Username = Encoding.UTF8.GetString(payload);

            //IsAccepted = !usernameListCopy.Contains(Username);

        }

        public bool GetAccepted() 
        {

            return IsAccepted; 
        
        }
    }
}
