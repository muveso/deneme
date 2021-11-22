using System;
using System.Collections.Generic;
using System.Net;
using Assets.Scripts.Network.Common;
using Assets.Scripts.Utils;
using Assets.Scripts.Utils.Network.TCP;
using Google.Protobuf;

namespace Assets.Scripts.Network.Server {
    public class TcpServerCommunicator : IServerCommunicator, IMessageReader, IDisposable {
        private readonly MessagesQueue _messagesQueue;
        private readonly TcpServerReceiverThread _tcpServerReceiverThread;

        public TcpServerCommunicator(IPEndPoint localEndPoint) {
            _messagesQueue = new MessagesQueue();
            Clients = new SynchronizedCollection<Common.Client>();
            Server = new TcpServer(localEndPoint);
            _tcpServerReceiverThread = new TcpServerReceiverThread(this);
            _tcpServerReceiverThread.Start();
        }

        public TcpServer Server { get; }

        public SynchronizedCollection<Common.Client> Clients { get; }


        public void Dispose() {
            _tcpServerReceiverThread?.Stop();
            Server?.Dispose();
        }

        public Message GetMessage() {
            return _messagesQueue.GetMessage();
        }

        public List<Message> GetAllMessages() {
            return _messagesQueue.GetAllMessages();
        }

        public void HostConnect(string nickname) {
            Clients.Add(new HostClient(new ClientDetails(nickname)));
        }

        public void AddMessage(Message message) {
            _messagesQueue.AddMessage(message);
        }

        public List<ClientDetails> GetClientDetailsList() {
            var clientDetailsList = new List<ClientDetails>();
            foreach (var client in Clients) {
                clientDetailsList.Add(client.Details);
            }

            return clientDetailsList;
        }

        public void SendToAllClients(IMessage message) {
            var messageBytes = MessagesHelpers.ConvertMessageToBytes(message);
            foreach (var client in Clients) {
                client.Send(messageBytes);
            }
        }
    }
}