using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using Assets.Scripts.Network.Common;
using Assets.Scripts.Utils;
using Assets.Scripts.Utils.Messages;
using Assets.Scripts.Utils.Network.TCP;
using Google.Protobuf;

namespace Assets.Scripts.Network.Server {
    public class TcpServerCommunicator : IServerCommunicatorForHost, IMessageReader, IServerMessageWriter, IDisposable {
        private readonly ConcurrentQueue<MessageToReceive> _receieveMessagesQueue;
        private readonly TcpServerCommunicatorReceiverThread _tcpServerCommunicatorReceiverThread;

        public TcpServerCommunicator(IPEndPoint localEndPoint) {
            _receieveMessagesQueue = new ConcurrentQueue<MessageToReceive>();
            Clients = new SynchronizedCollection<Common.Client>();
            Server = new TcpServer(localEndPoint);
            _tcpServerCommunicatorReceiverThread = new TcpServerCommunicatorReceiverThread(this);
            _tcpServerCommunicatorReceiverThread.Start();
        }

        public TcpServer Server { get; }

        public SynchronizedCollection<Common.Client> Clients { get; }


        public void Dispose() {
            _tcpServerCommunicatorReceiverThread?.Stop();
            Server?.Dispose();
        }

        public MessageToReceive Receive() {
            _receieveMessagesQueue.TryDequeue(out var message);
            return message;
        }

        public List<MessageToReceive> ReceiveAll() {
            return EnumerableUtils.DequeueAllQueue(_receieveMessagesQueue);
        }

        public void HostConnect(string nickname) {
            Clients.Add(new HostClient(new ClientDetails(nickname)));
        }

        public void AddMessageToReceive(MessageToReceive messageToReceive) {
            _receieveMessagesQueue.Enqueue(messageToReceive);
        }

        public void Send(IMessage message, List<IPEndPoint> clients) {
            throw new NotImplementedException();
        }

        public void SendAll(IMessage message) {
            throw new NotImplementedException();
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