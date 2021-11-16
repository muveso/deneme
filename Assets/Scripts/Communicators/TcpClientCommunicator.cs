using System;
using System.Collections.Concurrent;
using System.Net;
using Evade.Utils;
using Evade.Utils.Network;
using Google.Protobuf;

namespace Evade.Communicators {
    public interface IClientCommunicator : IDisposable {
        void Send(IMessage message);

        Message GetMessage();
    }

    public class TcpClientCommunicator : IClientCommunicator {
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

    public class HostClientCommunicator : IClientCommunicator {
        private readonly TcpServerCommunicator _tcpServerCommunicator;

        public HostClientCommunicator(TcpServerCommunicator tcpServerCommunicator, string nickname) {
            _tcpServerCommunicator = tcpServerCommunicator;
            _tcpServerCommunicator.HostConnect(nickname);
        }

        public void Send(IMessage message) {
            _tcpServerCommunicator.SendMessageFromHost(message);
        }

        public Message GetMessage() {
            return null;
        }

        public void Dispose() { }
    }
}