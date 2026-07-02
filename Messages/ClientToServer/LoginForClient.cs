using ServerForChatApp;
using System.Text;

namespace ServerForChatApp.Messages.ClientToServer
{
    internal class LoginForClient
    {
        public string Username { get; private set; }
        public int AssignedId { get; private set; }
        public bool IsAccepted { get; private set; }

        public LoginForClient(byte[] payload, RandomUserID idManager, UserDictionary userLogs)
        {
            Username = Encoding.UTF8.GetString(payload);
            AssignedId = idManager.GenerateRandomUserID(Username);
            userLogs.AddItem(Username, "0000-00-00-00:00:00");
            IsAccepted = userLogs.GetItem(Username) != null;
        }

        public byte[] ToBytes()
        {
            byte[] payload;
            if (IsAccepted)
            {
                payload = new byte[] { 0x01, (byte)AssignedId };
            }
            else
            {
                payload = new byte[] { 0x02 };
            }
            return payload;
        }
    }
}