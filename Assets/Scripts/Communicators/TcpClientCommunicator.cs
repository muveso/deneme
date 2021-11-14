using System;
using System.Collections.Concurrent;
using System.Net;
using Evade.Utils;
using Evade.Utils.Network;
using Google.Protobuf;

namespace Evade.Communicators {
    public class TcpClientCommunicator : IDisposable {
        private readonly TcpClientReceiverThread _tcpClientReceiverThread;

        public TcpClientCommunicator(string ipAddress, int port) {
            var serverEndPoint = new IPEndPoint(IPAddress.Parse(ipAddress), port);
            ClientGlobals.ServerEndpoint = serverEndPoint;
            MessagesQueue = new ConcurrentQueue<Message>();
            Client = new TcpClient(serverEndPoint);
            _tcpClientReceiverThread = new TcpClientReceiverThread(this);
            _tcpClientReceiverThread.Start();
        }

        public TcpClient Client { get; }

        public ConcurrentQueue<Message> MessagesQueue { get; }

        public void Dispose() {
            _tcpClientReceiverThread?.Stop();
            Client?.Dispose();
        }

        public void Send(IMessage message) {
            Client.Send(MessagesHelpers.ConvertMessageToBytes(message));
        }
    }
}