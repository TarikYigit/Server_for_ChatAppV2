using ServerForChatApp;
using System.Collections.Generic;

namespace Server_for_ChatApp.Messages.ServerInternals
{
    internal class MessageValid
    {
        public bool IsReceiverValid { get; private set; }
        public string ReceiverUsername { get; private set; } 

        public MessageValid(byte recieverId, Dictionary<int, string> userDictionary)
        {
            if (userDictionary.TryGetValue(recieverId, out string receiverUsername))
            {
                IsReceiverValid = true;
                ReceiverUsername = receiverUsername;
            }
            else
            {
                IsReceiverValid = false;
                ReceiverUsername = string.Empty;
            }
        }

        public bool GetIsReceiverValid()
        {
            return IsReceiverValid;
        }

        public string GetReceiverUsername()
        {
            return ReceiverUsername;
        }
    }
}