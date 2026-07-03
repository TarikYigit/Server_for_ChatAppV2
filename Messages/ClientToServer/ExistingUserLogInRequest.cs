using ServerForChatApp;
using System.Text;

namespace Server_for_ChatApp.Messages.ClientToServer
{
    internal class ExistingUserLogInRequest
    {
        public string Username { get; private set; }
        public int LoggedInUserId { get; private set; }
        public bool IsValid { get; private set; }

        public ExistingUserLogInRequest(byte[] payload, RandomUserID idManager)
        {
            Username = Encoding.UTF8.GetString(payload);
            IsValid = false;

            foreach (var kvp in idManager.UserIDDictionary)
            {
                if (kvp.Value == Username)
                {
                    LoggedInUserId = kvp.Key;
                    IsValid = true;
                    break;
                }
            }
        }

        public byte GetId()
        {
            return (byte)MessageId.LOG_IN;
        }

        public byte[] ToBytes()
        {
            if (IsValid)
            {
                return new byte[] { 0x01, (byte)LoggedInUserId };
            }
            else
            {
                return new byte[] { 0x02 };
            }
        }
    }
}