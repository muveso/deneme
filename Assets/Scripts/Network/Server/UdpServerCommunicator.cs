using System.Collections.Generic;
using System.Net;
using Assets.Scripts.Network.Common;
using Assets.Scripts.Utils;
using Google.Protobuf;

namespace Assets.Scripts.Network.Server {
    public class UdpServerCommunicator : AbstractUdpCommunicator, IServerCommunicatorForHost {
        private readonly SynchronizedCollection<IPEndPoint> _clients;
        private readonly UdpServerReceiverThread _udpServerReceiverThread;

        public UdpServerCommunicator(int listeningPort) : base(listeningPort) {
            _clients = new SynchronizedCollection<IPEndPoint>();
            _udpServerReceiverThread = new UdpServerReceiverThread(this);
            _udpServerReceiverThread.Start();
        }

        public void HostConnect(string nickname) { }

        public void InsertToQueue(Message message) {
            MessagesQueue.AddMessage(message);
        }

        public void AddClientIfNotExists(IPEndPoint endpoint) {
            if (!_clients.Contains(endpoint)) {
                _clients.Add(endpoint);
            }
        }

        public override void Dispose() {
            _udpServerReceiverThread?.Stop();
            base.Dispose();
        }

        public void SendToAllClients(IMessage message) {
            var messageBytes = MessagesHelpers.ConvertMessageToBytes(message);
            foreach (var client in _clients) {
                Client.SendTo(messageBytes, client);
            }
        }
    }
}