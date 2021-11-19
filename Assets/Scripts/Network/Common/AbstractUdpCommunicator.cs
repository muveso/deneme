using System.Collections.Concurrent;
using System.Net;
using Assets.Scripts.Utils;
using Assets.Scripts.Utils.Network.UDP;
using Google.Protobuf;

namespace Assets.Scripts.Network.Common {
    public abstract class AbstractUdpCommunicator : IClientCommunicator {
        protected AbstractUdpCommunicator(IPEndPoint endpoint) {
            MessagesQueue = new ConcurrentQueue<Message>();
            Client = new UdpClient(endpoint);
        }

        protected AbstractUdpCommunicator(int listenPort) {
            MessagesQueue = new ConcurrentQueue<Message>();
            Client = new UdpClient(listenPort);
        }

        public UdpClient Client { get; }

        public ConcurrentQueue<Message> MessagesQueue { get; }

        public virtual void Dispose() {
            Client.Dispose();
        }

        public Message GetMessage() {
            Message message;
            MessagesQueue.TryDequeue(out message);
            return message;
        }

        public void Send(IMessage message) {
            Client.SendAsync(MessagesHelpers.ConvertMessageToBytes(message));
        }
    }
}