using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Assets.Scripts.Utils {
    public class MessagesQueue {
        private readonly ConcurrentQueue<Message> _queue;

        public MessagesQueue() {
            _queue = new ConcurrentQueue<Message>();
        }

        public Message GetMessage() {
            _queue.TryDequeue(out var message);
            return message;
        }

        public List<Message> GetAllMessages() {
            var messages = new List<Message>();
            while (!_queue.IsEmpty) {
                if (_queue.TryDequeue(out var message)) {
                    messages.Add(message);
                }
            }

            return messages;
        }

        public void AddMessage(Message message) {
            _queue.Enqueue(message);
        }
    }
}