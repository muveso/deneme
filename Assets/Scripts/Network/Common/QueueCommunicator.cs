using System.Collections.Concurrent;
using System.Collections.Generic;
using Assets.Scripts.Utils;
using Assets.Scripts.Utils.Messages;
using Google.Protobuf;

namespace Assets.Scripts.Network.Common {
    public class QueueCommunicator {
        private readonly ConcurrentQueue<MessageToReceive> _receiveMessagesQueue;
        private readonly ConcurrentQueue<MessageToSend> _sendMessagesQueue;

        public QueueCommunicator() {
            _receiveMessagesQueue = new ConcurrentQueue<MessageToReceive>();
            _sendMessagesQueue = new ConcurrentQueue<MessageToSend>();
        }

        public MessageToReceive Receive() {
            return EnumerableUtils.TryDequeue(_receiveMessagesQueue);
        }

        public List<MessageToReceive> ReceiveAll() {
            return EnumerableUtils.DequeueAllQueue(_receiveMessagesQueue);
        }

        public void Send(IMessage message) {
            _sendMessagesQueue.Enqueue(new MessageToSend(null, message));
        }

        public void Send(MessageToSend message) {
            _sendMessagesQueue.Enqueue(message);
        }

        public MessageToSend GetMessageToSend() {
            return EnumerableUtils.TryDequeue(_sendMessagesQueue);
        }

        public void AddMessageToReceive(MessageToReceive messageToReceive) {
            _receiveMessagesQueue.Enqueue(messageToReceive);
        }
    }
}