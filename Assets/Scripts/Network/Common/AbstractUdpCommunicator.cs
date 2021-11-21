using System.Collections.Generic;
using System.Net;
using Assets.Scripts.Utils;
using Assets.Scripts.Utils.Network.UDP;
using Google.Protobuf;

namespace Assets.Scripts.Network.Common {
    public abstract class AbstractUdpCommunicator : IClientCommunicator {
        protected AbstractUdpCommunicator(IPEndPoint endpoint) {
            MessagesQueue = new MessagesQueue();
            Client = new UdpClient(endpoint);
        }

        protected AbstractUdpCommunicator(int listenPort) {
            MessagesQueue = new MessagesQueue();
            Client = new UdpClient(listenPort);
        }

        public UdpClient Client { get; }

        public MessagesQueue MessagesQueue { get; }

        public virtual void Dispose() {
            Client.Dispose();
        }

        public Message Receive() {
            return MessagesQueue.GetMessage();
        }


        public void Send(IMessage message) {
            Client.SendAsync(MessagesHelpers.ConvertMessageToBytes(message));
        }

        public List<Message> ReceiveAll() {
            return MessagesQueue.GetAllMessages();
        }
    }
}