using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using Assets.Scripts.Utils;
using Assets.Scripts.Utils.Messages;
using Google.Protobuf;

namespace Assets.Scripts.Network.Common {
    public class QueueCommunicator {
        private readonly BlockingCollection<MessageToReceive> _receiveMessagesQueue;
        private readonly BlockingCollection<MessageToSend> _sendMessagesQueue;

        public QueueCommunicator() {
            _receiveMessagesQueue = new BlockingCollection<MessageToReceive>(new ConcurrentQueue<MessageToReceive>());
            _sendMessagesQueue = new BlockingCollection<MessageToSend>(new ConcurrentQueue<MessageToSend>());
        }

        public MessageToReceive Receive(int millisecondsTimeout = 0) {
            return EnumerableUtils.TryDequeue(_receiveMessagesQueue, millisecondsTimeout);
        }

        public List<MessageToReceive> ReceiveAll() {
            return EnumerableUtils.DequeueAllQueue(_receiveMessagesQueue);
        }

        public void Send(IMessage message, List<IPEndPoint> ipEndpoints = null) {
            _sendMessagesQueue.Add(new MessageToSend(ipEndpoints, message));
        }

        public MessageToSend GetMessageToSend(int millisecondsTimeout = 0) {
            return EnumerableUtils.TryDequeue(_sendMessagesQueue, millisecondsTimeout);
        }

        public void AddMessageToReceive(MessageToReceive messageToReceive) {
            _receiveMessagesQueue.Add(messageToReceive);
        }
    }
}