//using ServerForChatApp.Messages.ClientToServer;
//using ServerForChatApp.Messages.ClientToServer;
//using ServerForChatApp;
//using System;
//using System.Collections.Generic;
//using System.Net.Sockets;
//using System.Text;

//namespace ServerForChatApp.Messages.ServerToClient
//{
//    internal class SendMessageToClient
//    {
//        public static void SendMessageFromClientToClient(TCPServer server, NetworkStream stream, RandomUserID idManager, SendMessageRequestFromClient request, Dictionary<string, NetworkStream> ActiveConnections)
//        {

//            byte senderId = request.Sender;
//            byte receiverId = request.Reciever;
//            byte[] messageBytes = request.Message;

//            if (idManager.UserIDDictionary.TryGetValue(receiverId, out string receiverUsername))
//            {

//                if (ActiveConnections.TryGetValue(receiverUsername, out NetworkStream receiverStream))  //Online 
//                {

//                    byte[] payload = new byte[1 + messageBytes.Length];
//                    payload[0] = senderId;
//                    Array.Copy(messageBytes, 0, payload, 1, messageBytes.Length);

//                    server.SendPacket(receiverStream, 0x03, payload);
//                }
//                else    //Offline
//                {
//                    NewMessageLog.AddNewMessage($"[{DateTime.Now.ToString("yyyy-MM-dd-HH:mm:ss")}] {senderId} {receiverId} {Encoding.UTF8.GetString(messageBytes)}\n");
//                }
//            }
//        }
//    }
//}