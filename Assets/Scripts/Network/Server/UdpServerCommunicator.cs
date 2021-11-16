using System.Collections.Generic;
using System.Net;
using Assets.Scripts.Network.Common;
using Assets.Scripts.Utils;
using Google.Protobuf;

namespace Assets.Scripts.Network.Server {
    public class UdpServerCommunicator : AbstractUdpClientCommunicator {
        private readonly UdpServerReceiverThread _udpServerReceiverThread;

        public UdpServerCommunicator(int listeningPort) : base(listeningPort) {
            Clients = new SynchronizedCollection<IPEndPoint>();
            _udpServerReceiverThread = new UdpServerReceiverThread(this);
            _udpServerReceiverThread.Start();
        }

        public SynchronizedCollection<IPEndPoint> Clients { get; }

        public override void Dispose() {
            _udpServerReceiverThread?.Stop();
            base.Dispose();
        }

        public void SendToAllClients(IMessage message) {
            var messageBytes = MessagesHelpers.ConvertMessageToBytes(message);
            foreach (var client in Clients) {
                Client.SendTo(messageBytes, client);
            }
        }
    }
}