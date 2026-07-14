using Server_for_ChatApp.Interfaces.RequestInterfaces;
using ServerForChatApp;
using System.Text;

namespace Server_for_ChatApp.Messages.ClientToServer
{
    internal class RegisterRequest : IRequest
    {   
        private string Username { get;  set; }

        private string Password { get;  set; }

        public RegisterRequest(byte[] payload)
        {
            using (MemoryStream ms = new MemoryStream(payload))

            using (BinaryReader reader = new BinaryReader(ms))
            {
                int userLen = reader.ReadByte();

                byte[] userBytes = reader.ReadBytes(userLen);

                Username = Encoding.UTF8.GetString(userBytes);

                int passLen = reader.ReadByte();

                byte[] passBytes = reader.ReadBytes(passLen);

                Password = Encoding.UTF8.GetString(passBytes);

            }
        }

        public string GetUsername()
        {

            return Username;

        }

        public string GetPassword() 
        { 

            return Password; 

        }

        public byte GetId()
        {

            return (byte)MessageId.REGISTER;

        }
    }
}
