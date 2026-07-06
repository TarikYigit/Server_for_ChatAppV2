using System.Collections.Generic;
using System.Linq;

namespace ServerForChatApp
{
    internal class GetCopyOfUserDictionaryOnlyNames
    {
        public List<string> GetUsernames(Dictionary<string, string> internalDictionary)
        {
            if (internalDictionary == null)
            {
                return new List<string>();
            }

            return internalDictionary.Keys.ToList();
        }
    }
}