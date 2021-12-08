using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using Assets.Scripts.Network.Common;
using Assets.Scripts.Utils;
using Assets.Scripts.Utils.Messages;
using Assets.Scripts.Utils.Network.TCP;
using Google.Protobuf;

namespace Assets.Scripts.Network.Server {
    public class TcpServerCommunicator : IServerCommunicatorForHost, IDisposable {
        private readonly ConcurrentQueue<MessageToReceive> _receieveMessagesQueue;
        private readonly ConcurrentQueue<MessageToSend> _sendMessagesQueue;
        private readonly TcpServerCommunicatorReceiverThread _tcpServerCommunicatorReceiverThread;
        private readonly TcpServerCommunicatorSenderThread _tcpServerCommunicatorSenderThread;

        public TcpServerCommunicator(IPEndPoint localEndPoint) {
            _receieveMessagesQueue = new ConcurrentQueue<MessageToReceive>();
            _sendMessagesQueue = new ConcurrentQueue<MessageToSend>();
            Clients = new SynchronizedCollection<Common.Client>();
            Server = new TcpServer(localEndPoint);
            _tcpServerCommunicatorSenderThread = new TcpServerCommunicatorSenderThread(this);
            _tcpServerCommunicatorSenderThread.Start();
            _tcpServerCommunicatorReceiverThread = new TcpServerCommunicatorReceiverThread(this);
            _tcpServerCommunicatorReceiverThread.Start();
        }

        public TcpServer Server { get; }

        public SynchronizedCollection<Common.Client> Clients { get; }


        public void Dispose() {
            _tcpServerCommunicatorReceiverThread?.Stop();
            _tcpServerCommunicatorSenderThread?.Stop();
            Server?.Dispose();
        }

        public void HostConnect(string nickname) {
            Clients.Add(new HostClient(new ClientDetails(nickname)));
        }

        public void AddMessageToReceive(MessageToReceive messageToReceive) {
            _receieveMessagesQueue.Enqueue(messageToReceive);
        }

        public MessageToReceive Receive() {
            return EnumerableUtils.TryDequeue(_receieveMessagesQueue);
        }

        public List<MessageToReceive> ReceiveAll() {
            return EnumerableUtils.DequeueAllQueue(_receieveMessagesQueue);
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