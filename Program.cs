using System;
using System.ComponentModel.Design;
using System.Net;
using System.Net.Sockets;
using System.Reflection.Metadata;
using System.Text;
namespace Server_for_ChatApp
{
    class TCPServer
    {
        public static string file = @"C:\Users\tarik.dalkiran\Desktop\Workspace\Playground\Message_Save.txt"; //used in send data stuff

        private TcpListener listener;
        private bool isRunning;
        UserDictionary User_Logs = new UserDictionary();
        public TCPServer(int port)
        {
            listener = new TcpListener(IPAddress.Any, port);
        }
        public void Start()
        {
            listener.Start(); //start listening for incoming connections
            isRunning = true;

            Console.WriteLine("Server started. Waiting for connections..."); //server active check
            while (isRunning)
            {
                TcpClient client = listener.AcceptTcpClient();
                Console.WriteLine("Client connected."); //client check
                HandleClient(client);
            }
        }
        public void HandleClient(TcpClient client)
        {
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[1024];
            int bytesRead;

            while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0) // I am assuming you dont send data upon opening the message app
            {
                string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                if (message.Split(" ")[1] == "99")
                {
                    User_Logs.AddItem(message.Split(" ")[0], "0000-00-00-00:00:00"); // add new user, hasnt logged out yet (use 99 as identifier of new connection)
                }
                else if (message.Split(" ")[1] == "11")
                {
                    User_Logs.SetItem(message.Split(" ")[0], DateTime.Now.ToString("yyyy-MM-dd-HH:mm:ss")); //user logged out, set log out time (use 11 as identifier of log out)
                }
                else
                {
                    New_Message_Log.Add_New_Message(message); //save message to file
                }
            }
            
            Console.WriteLine("Client disconnected.");
            client.Close();
        }

        public static void Main(string[] args)
        {
            int port = 5000; // You can change the port number if needed
            TCPServer server = new TCPServer(port);
            server.Start();
        }
    }
}