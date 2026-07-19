namespace ServerForChatApp
{
    internal static class CheckOfflineMessages
    {

        public static string file = @"C:\Users\Tarık\Desktop\Message_Save.txt";

        public static List<string> GetAndRemoveMessages(int user_ID)
        {

            List<string> userMessages = new List<string>();

            List<string> linesToKeep = new List<string>();

            if (!File.Exists(file)) return userMessages;

            foreach (var line in File.ReadLines(file))
            {

                string[] parts = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length >= 4 && parts[2] == user_ID.ToString())
                {

                    userMessages.Add(line);

                }
                else
                {

                    linesToKeep.Add(line);

                }
            }

            File.WriteAllLines(file, linesToKeep);

            return userMessages;

        }
    }
}