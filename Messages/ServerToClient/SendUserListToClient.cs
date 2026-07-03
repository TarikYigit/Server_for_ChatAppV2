//using ServerForChatApp.Messages.ServerToClient;
//using ServerForChatApp.Messages.ClientToServer;
//using ServerForChatApp;
//using System.Net.Sockets;

//namespace ServerForChatApp.Messages.ServerToClient
//{
//    internal class SendUserListToClient
//    {
//        public static void SendUserListPacket(TCPServer server, NetworkStream stream, GetUserListForClient listData)
//        {
//            server.SendPacket(stream, 0x02, listData.UserListPayload);
//        }
//    }
//}