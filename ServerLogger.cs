namespace ServerForChatApp
{
    public static class ServerLogger
    {
        public static void LogAuth(string message) => Print("AUTH", message, ConsoleColor.Green);
        public static void LogRoute(string message) => Print("ROUTE", message, ConsoleColor.Cyan);
        public static void LogVault(string message) => Print("VAULT", message, ConsoleColor.DarkYellow);
        public static void LogError(string message) => Print("ERROR", message, ConsoleColor.Red);
        public static void LogNetwork(string message) => Print("NETWORK", message, ConsoleColor.DarkGray);

        private static void Print(string tag, string message, ConsoleColor color)
        {

            Console.Write("[");

            Console.ForegroundColor = color;

            Console.Write(tag.PadRight(7)); 

            Console.ResetColor();

            Console.WriteLine($"] {DateTime.Now:HH:mm:ss.fff} | {message}");

        }
    }
}