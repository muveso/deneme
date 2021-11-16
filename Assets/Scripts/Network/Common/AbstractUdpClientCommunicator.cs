using System;
using System.Collections.Concurrent;
using Assets.Scripts.Utils;
using Assets.Scripts.Utils.Network.UDP;
using Google.Protobuf;

namespace Assets.Scripts.Network.Common {
    public abstract class AbstractUdpClientCommunicator : IDisposable {
        protected AbstractUdpClientCommunicator(string ipAddress, int port) {
            MessagesQueue = new ConcurrentQueue<Message>();
            Client = new UdpClient(ipAddress, port);
        }

        protected AbstractUdpClientCommunicator(int listenPort) {
            MessagesQueue = new ConcurrentQueue<Message>();
            Client = new UdpClient(listenPort);
        }

        public UdpClient Client { get; }

        public ConcurrentQueue<Message> MessagesQueue { get; }

        public virtual void Dispose() {
            Client.Dispose();
        }

        public void Send(IMessage message) {
            Client.Send(MessagesHelpers.ConvertMessageToBytes(message));
        }

        public Message TryGetMessageFromQueue() {
            Message message;
            MessagesQueue.TryDequeue(out message);
            return message;
        }
    }
}