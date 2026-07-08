using Server_for_ChatApp.Interfaces.RequestInterfaces;
using ServerForChatApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server_for_ChatApp.Messages.ClientToServer
{
    internal class LoginRequest : IRequest
    {   
        public string Username { get; private set; }
        public LoginRequest(byte[] payload)
        {

            Username = Encoding.UTF8.GetString(payload);

        }

        public string GetUsername()
        {

            return Username;

        }
    }
}
