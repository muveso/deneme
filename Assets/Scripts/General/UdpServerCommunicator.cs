using System.Collections.Generic;
using System.Net;
using Evade.Utils;
using Google.Protobuf;

namespace Evade {
    public class UdpServerCommunicator : UdpCommunicator {
        public UdpServerCommunicator(int listeningPort,
            List<IPEndPoint> clients) : base(listeningPort) {
            Clients = clients;
        }

        public List<IPEndPoint> Clients { get; }

        public void SendToAllClients(IMessage message) {
            var messageBytes = MessagesHelpers.ConvertMessageToBytes(message);
            foreach (var client in Clients) {
                _client.SendTo(messageBytes, client);
            }
        }
    }
}