using System.Collections.Generic;

namespace ServerForChatApp
{
    internal class GetCopyOfUserDictionaryAll
    {
        public Dictionary<int, string> GetSafeFilteredUsers(Dictionary<int, string> sourceDictionary, byte requesterId)
        {
            Dictionary<int, string> filteredUsers = new Dictionary<int, string>();

            if (sourceDictionary != null)
            {

                lock (sourceDictionary)
                {

                    foreach (var kvp in sourceDictionary)
                    {

                        if (kvp.Key != requesterId)
                        {

                            filteredUsers.Add(kvp.Key, kvp.Value);

                        }
                    }
                }
            }

            return filteredUsers;
        }
    }
}