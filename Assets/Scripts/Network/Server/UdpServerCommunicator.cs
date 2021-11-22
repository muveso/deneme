using System;
using System.Collections.Generic;
using System.Net;
using Assets.Scripts.Network.Client;
using Assets.Scripts.Network.Common;
using Assets.Scripts.Utils;
using Assets.Scripts.Utils.Network.UDP;
using Google.Protobuf;

namespace Assets.Scripts.Network.Server {
    public class UdpServerCommunicator : IServerCommunicator, IMessageReader, IDisposable {
        private readonly UdpClientMessageBasedClient _client;
        private readonly SynchronizedCollection<IPEndPoint> _clients;
        private readonly MessagesQueue _messagesQueue;
        private readonly UdpServerReceiverThread _udpServerReceiverThread;

        public UdpServerCommunicator(int listeningPort) {
            _messagesQueue = new MessagesQueue();
            _client = new UdpClientMessageBasedClient(new UdpClient(listeningPort));
            _clients = new SynchronizedCollection<IPEndPoint>();
            _udpServerReceiverThread = new UdpServerReceiverThread(this);
            _udpServerReceiverThread.Start();
        }

        public void Dispose() {
            _udpServerReceiverThread?.Stop();
            _client?.Dispose();
        }

        public Message GetMessage() {
            return _messagesQueue.GetMessage();
        }

        public List<Message> GetAllMessages() {
            return _messagesQueue.GetAllMessages();
        }

        public void HostConnect(string nickname) { }

        public void AddMessage(Message message) {
            _messagesQueue.AddMessage(message);
        }

        public void AddClientIfNotExists(IPEndPoint endpoint) {
            if (!_clients.Contains(endpoint)) {
                _clients.Add(endpoint);
            }
        }

        public Message Receive(bool block = true) {
            return _client.Receive(block);
        }

        public void SendToAllClients(IMessage message) {
            foreach (var client in _clients) {
                _client.SendTo(message, client);
            }
        }
    }
}