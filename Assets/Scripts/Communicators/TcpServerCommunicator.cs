using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Evade.Utils;
using Evade.Utils.Network;
using Google.Protobuf;

namespace Evade.Communicators {
    public class TcpServerCommunicator : IDisposable {
        private readonly TcpServerReceiverThread _tcpServerReceiverThread;

        public TcpServerCommunicator(string ipAddress, int listeningPort) {
            MessagesQueue = new ConcurrentQueue<Message>();
            Clients = new SynchronizedCollection<Client>();
            Server = new TcpServer(ipAddress, listeningPort);
            _tcpServerReceiverThread = new TcpServerReceiverThread(this);
            _tcpServerReceiverThread.Start();
        }

        public TcpServer Server { get; }

        public SynchronizedCollection<Client> Clients { get; }

        public ConcurrentQueue<Message> MessagesQueue { get; }

        public void Dispose() {
            _tcpServerReceiverThread?.Stop();
            Server?.Dispose();
        }

        public void SendToAllClients(IMessage message) {
            var messageBytes = MessagesHelpers.ConvertMessageToBytes(message);
            foreach (var client in Clients) {
                client.TcpClient.Send(messageBytes);
            }
        }
    }

    public class Client {
        public Client(TcpClient tcpClient) {
            TcpClient = tcpClient;
            Details = new ClientDetails();
        }

        public ClientDetails Details { get; }
        public TcpClient TcpClient { get; }

        public override string ToString() {
            return $"IP: {TcpClient} | {Details}";
        }
    }
}