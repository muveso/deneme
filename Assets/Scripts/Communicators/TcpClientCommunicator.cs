using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using Evade.Utils;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using TcpClient = Evade.Utils.Network.TcpClient;

namespace Evade.Communicators {
    public class TcpClientCommunicator : BaseThread, IDisposable {
        private const int PollTimeoutMs = 1000;
        private readonly TcpClient _client;

        public TcpClientCommunicator(string ipAddress, int port) {
            var serverEndPoint = new IPEndPoint(IPAddress.Parse(ipAddress), port);
            ClientGlobals.ServerEndpoint = serverEndPoint;
            MessagesQueue = new ConcurrentQueue<Any>();
            _client = new TcpClient(serverEndPoint);
        }

        public ConcurrentQueue<Any> MessagesQueue { get; }

        public void Dispose() {
            Stop();
            _client.Dispose();
        }

        public void Send(IMessage message) {
            _client.Send(MessagesHelpers.ConvertMessageToBytes(message));
        }

        protected override void RunThread() {
            while (ThreadShouldRun) {
                if (_client.Sock.Poll(PollTimeoutMs, SelectMode.SelectRead)) {
                    HandleMessage();
                }
            }
        }

        private void HandleMessage() {
            var messageBytes = _client.Recieve();
            var message = MessagesHelpers.ConvertBytesToMessage(messageBytes);
            MessagesQueue.Enqueue(message);
        }
    }
}