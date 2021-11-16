using System.Collections.Concurrent;
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
            MessagesQueue = new ConcurrentQueue<Message>();
            Client = new TcpClient(serverEndPoint);
            _tcpClientReceiverThread = new TcpClientReceiverThread(this);
            _tcpClientReceiverThread.Start();
        }

        public TcpClient Client { get; }

        public ConcurrentQueue<Message> MessagesQueue { get; }

        public void Send(IMessage message) {
            Client.Send(MessagesHelpers.ConvertMessageToBytes(message));
        }

        public Message GetMessage() {
            MessagesQueue.TryDequeue(out var message);
            return message;
        }

        public void Dispose() {
            _tcpClientReceiverThread?.Stop();
            Client?.Dispose();
        }
    }
}