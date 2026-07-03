//using ServerForChatApp.Messages.ClientToServer;
//using ServerForChatApp;
//using System;
//using System.Net.Sockets;
//using System.Text;

//namespace ServerForChatApp.Messages.ServerToClient
//{
//    internal class SendOfflineMessagesToClient
//    {
//        public static void SendMessages(TCPServer server, NetworkStream stream, FetchOfflineMessagesForClient request)
//        {
//            byte requesterId = request.RequesterId;

//            Console.WriteLine($"\n[SERVER] User {requesterId} is asking for offline messages...");
//            Console.WriteLine($"[SERVER] Found {request.OfflineMessages.Count} messages in the vault for User {requesterId}.");

//            foreach (string rawFileLine in request.OfflineMessages)
//            {
//                string[] parts = rawFileLine.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

//                byte realSenderId = 0;
//                int messageStartIndex = -1;
//                for (int i = 0; i < parts.Length - 1; i++)
//                {
//                    if (byte.TryParse(parts[i], out byte parsedSender) &&
//                        byte.TryParse(parts[i + 1], out byte parsedReceiver))
//                    {
//                        if (parsedReceiver == requesterId)
//                        {
//                            realSenderId = parsedSender;
//                            messageStartIndex = i + 2;
//                            break;
//                        }
//                    }
//                }

//                if (messageStartIndex != -1)
//                {
//                    string actualMessage = "";
//                    for (int i = messageStartIndex; i < parts.Length; i++)
//                    {
//                        actualMessage += parts[i] + (i == parts.Length - 1 ? "" : " ");
//                    }

//                    Console.WriteLine($"[SERVER] Packaging message from ID {realSenderId} to ID {requesterId}. Content: {actualMessage}");

//                    byte[] msgBytes = Encoding.UTF8.GetBytes(actualMessage);
//                    byte[] payload = new byte[1 + msgBytes.Length];
//                    payload[0] = realSenderId;
//                    Array.Copy(msgBytes, 0, payload, 1, msgBytes.Length);

//                    server.SendPacket(stream, 0x03, payload);

//                    Console.WriteLine($"[SERVER] Packet sent to User {requesterId} successfully");
//                }
//                else
//                {
//                    Console.WriteLine($"[SERVER-ERROR] Failed to parse IDs from line: {rawFileLine}");
//                }
//            }
//        }
//    }
//}