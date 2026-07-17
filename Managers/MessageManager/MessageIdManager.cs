using System;
using System.Collections.Concurrent;

namespace Server_for_ChatApp.Managers.MessageManager
{

    public class TrackedMessage
    {

        public int MessageId { get; set; }

        public byte SenderId { get; set; }

        public byte ReceiverId { get; set; }

    }

    internal class MessageIdManager
    {
        private Random _random;

        private ConcurrentDictionary<int, TrackedMessage> _messageRegistry;

        public MessageIdManager()
        {

            _random = new Random();

            _messageRegistry = new ConcurrentDictionary<int, TrackedMessage>();

        }

        public int GenerateUniqueMessageId()
        {

            while (true)
            {

                int randomNumber = _random.Next(1, int.MaxValue);

                if (!_messageRegistry.ContainsKey(randomNumber))
                {

                    return randomNumber;

                }
            }
        }

        public void RegisterMessage(TrackedMessage message)
        {

            _messageRegistry.TryAdd(message.MessageId, message);

        }

        public TrackedMessage GetMessageById(int messageId)
        {

            if (_messageRegistry.TryGetValue(messageId, out TrackedMessage foundMessage))
            {

                return foundMessage;

            }

            return null;
        }

        public void RemoveMessage(int messageId)
        {

            _messageRegistry.TryRemove(messageId, out _);

        }
    }
}