using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Evade.Utils;
using Evade.Utils.Network;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;

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

        public List<ClientDetails> GetClientDetailsList() {
            var clientDetailsList = new List<ClientDetails>();
            foreach (var client in Clients) {
                clientDetailsList.Add(client.Details);
            }

            return clientDetailsList;
        }

        public void SendMessageFromHost(IMessage message) {
            MessagesQueue.Enqueue(new Message(HostClient.GetHostClientEndpoint(), Any.Pack(message)));
        }

        public void HostConnect(string nickname) {
            Clients.Add(new HostClient(new ClientDetails(nickname)));
        }

        public void SendToAllClients(IMessage message) {
            var messageBytes = MessagesHelpers.ConvertMessageToBytes(message);
            foreach (var client in Clients) {
                client.Send(messageBytes);
            }
        }
    }
}