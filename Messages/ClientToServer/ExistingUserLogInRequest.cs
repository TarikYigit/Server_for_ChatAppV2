using ServerForChatApp;
using System.Text;

namespace ServerForChatApp.Messages.ClientToServer
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
    }
}