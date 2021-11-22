using System.Collections.Generic;
using Assets.Scripts.Network.Common;
using Assets.Scripts.Utils;
using Google.Protobuf;

namespace Assets.Scripts.Network.Client {
    public class NetworkClientCommunicator : IClientCommunicator, IMessageWriter {
        private readonly IMessageBasedClient _client;
        private readonly MessagesQueue _messagesQueue;
        private readonly NetworkClientReceiverThread _networkClientReceiverThread;

        public NetworkClientCommunicator(IMessageBasedClient messageBasedClient) {
            _messagesQueue = new MessagesQueue();
            _client = messageBasedClient;
            _networkClientReceiverThread = new NetworkClientReceiverThread(this);
            _networkClientReceiverThread.Start();
        }

        public void Dispose() {
            _networkClientReceiverThread?.Stop();
            _client?.Dispose();
        }

        public void Send(IMessage message) {
            _client.Send(message);
        }

        public Message Receive(bool block = true) {
            return _client.Receive(block);
        }

        public Message GetMessage() {
            return _messagesQueue.GetMessage();
        }

        public List<Message> GetAllMessages() {
            return _messagesQueue.GetAllMessages();
        }

        public void AddMessage(Message message) {
            _messagesQueue.AddMessage(message);
        }
    }
}