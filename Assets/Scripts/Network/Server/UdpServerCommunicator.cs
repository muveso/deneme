using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using Assets.Scripts.Network.Client;
using Assets.Scripts.Utils;
using Assets.Scripts.Utils.Messages;
using Assets.Scripts.Utils.Network.UDP;
using Google.Protobuf;

namespace Assets.Scripts.Network.Server {
    public class UdpServerCommunicator : IServerCommunicatorForHost, IDisposable {
        private readonly ConcurrentQueue<MessageToReceive> _receiveMessagesQueue;
        private readonly ConcurrentQueue<MessageToSend> _sendMessagesQueue;
        private readonly UdpServerCommunicatorReceiverThread _udpServerCommunicatorReceiverThread;
        private readonly UdpServerCommunicatorSenderThread _udpServerCommunicatorSenderThread;

        public UdpServerCommunicator(int listeningPort) {
            _receiveMessagesQueue = new ConcurrentQueue<MessageToReceive>();
            _sendMessagesQueue = new ConcurrentQueue<MessageToSend>();
            Client = new UdpClientMessageBasedClient(new UdpClient(listeningPort));
            Clients = new SynchronizedCollection<IPEndPoint>();
            _udpServerCommunicatorSenderThread = new UdpServerCommunicatorSenderThread(this);
            _udpServerCommunicatorSenderThread.Start();
            _udpServerCommunicatorReceiverThread = new UdpServerCommunicatorReceiverThread(this);
            _udpServerCommunicatorReceiverThread.Start();
        }

        public UdpClientMessageBasedClient Client { get; }
        public SynchronizedCollection<IPEndPoint> Clients { get; }

        public void Dispose() {
            _udpServerCommunicatorSenderThread?.Stop();
            _udpServerCommunicatorReceiverThread?.Stop();
            Client?.Dispose();
        }

        public void HostConnect(string nickname) { }

        public void AddMessageToReceive(MessageToReceive messageToReceive) {
            _receiveMessagesQueue.Enqueue(messageToReceive);
        }

        public MessageToReceive Receive() {
            return EnumerableUtils.TryDequeue(_receiveMessagesQueue);
        }

        public List<MessageToReceive> ReceiveAll() {
            return EnumerableUtils.DequeueAllQueue(_receiveMessagesQueue);
        }

        public void Send(IMessage message, List<IPEndPoint> clients) {
            _sendMessagesQueue.Enqueue(new MessageToSend(clients, message));
        }

        public void SendAll(IMessage message) {
            Send(message, null);
        }

        public MessageToSend GetMessageToSend() {
            return EnumerableUtils.TryDequeue(_sendMessagesQueue);
        }
    }
}