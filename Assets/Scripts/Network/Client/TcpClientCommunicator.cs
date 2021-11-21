using System.Collections.Generic;
using System.Net;
using Assets.Scripts.General;
using Assets.Scripts.Network.Common;
using Assets.Scripts.Utils;
using Assets.Scripts.Utils.Network.TCP;
using Google.Protobuf;

namespace Assets.Scripts.Network.Client {
    public class TcpClientCommunicator : IClientCommunicator {
        private readonly TcpClientReceiverThread _tcpClientReceiverThread;

        public TcpClientCommunicator(IPEndPoint serverEndPoint) {
            NetworkManager.Instance.ServerIpAddress = serverEndPoint.Address;
            MessagesQueue = new MessagesQueue();
            Client = new TcpClient(serverEndPoint);
            _tcpClientReceiverThread = new TcpClientReceiverThread(this);
            _tcpClientReceiverThread.Start();
        }

        public TcpClient Client { get; }

        public MessagesQueue MessagesQueue { get; }

        public void Send(IMessage message) {
            Client.Send(MessagesHelpers.ConvertMessageToBytes(message));
        }

        public Message Receive() {
            return MessagesQueue.GetMessage();
        }

        public List<Message> ReceiveAll() {
            return MessagesQueue.GetAllMessages();
        }

        public void Dispose() {
            _tcpClientReceiverThread?.Stop();
            Client?.Dispose();
        }
    }
}