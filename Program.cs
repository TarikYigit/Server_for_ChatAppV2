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
        UserDictionary User_Logs = new UserDictionary();
        public TCPServer(int port)
        {
            // "127.0.0.1"
            listener = new TcpListener(IPAddress.Loopback, port);
        }
        public void Start()
        {
            listener.Start(); //start listening for incoming connections

            Console.WriteLine("Server started. Waiting for connections..."); //server active check
            while (true)
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

            while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0) 
            {
                string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                if (message.Split(" ")[1] == "99")
                {
                    User_Logs.AddItem(message.Split(" ")[0], "0000-00-00-00:00:00");                      // add new user, hasnt logged out yet 

                    //    CheckUser.Set_User_Active(message.Split(" ")[0]); //set user active
                    //else //send message back saying already exists and reject
                    //string rejection_message = "User exists";
                    //byte[] rejected = Encoding.UTF8.GetBytes(rejection_message);
                    //stream.Write(rejected, 0, rejected.Length);

                }
                else if (message.Split(" ")[1] == "11")
                {
                    User_Logs.SetItem(message.Split(" ")[0], DateTime.Now.ToString("yyyy-MM-dd-HH:mm:ss")); //user logged out, set log out time 
                    //CheckUser.Set_User_Offline(message.Split(" ")[0]);
                }
                else if (message.Split(" ")[1] == "to")
                {
                    //Room_Dictionary. //set room which user wants to look at
                }
                else
                {
                    New_Message_Log.Add_New_Message(message + "\n"); //save message to file
                }
            }
            
            client.Close();
        }

        public static void Main()
        {
            int port = 5000; 
            TCPServer server = new TCPServer(port);
            server.Start();
        }
    }
}