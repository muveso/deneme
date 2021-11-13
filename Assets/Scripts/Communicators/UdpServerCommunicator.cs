using System.Collections.Generic;
using System.Net;
using Evade.Utils;
using Google.Protobuf;

namespace Evade.Communicators {
    public class UdpServerCommunicator : UdpCommunicator {
        public UdpServerCommunicator(int listeningPort) : base(listeningPort) {
            Clients = new SynchronizedCollection<IPEndPoint>();
        }

        public SynchronizedCollection<IPEndPoint> Clients { get; }

        protected override void PreHandleMessage(IPEndPoint endPoint, byte[] messageBytes) {
            if (!Clients.Contains(endPoint)) {
                Clients.Add(endPoint);
            }
        }

        public void SendToAllClients(IMessage message) {
            var messageBytes = MessagesHelpers.ConvertMessageToBytes(message);
            foreach (var client in Clients) {
                Client.SendTo(messageBytes, client);
            }
        }
    }
}