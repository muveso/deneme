using System.Collections.Generic;
using System.Net;
using Evade.Utils;

namespace Evade.Game {
    public class UdpServerCommunicator : UdpCommunicator {
        public SynchronizedCollection<IPEndPoint> Clients { get; private set; }

        public void SendToAllClients(Google.Protobuf.IMessage message) {
            byte[] messageBytes = MessagesHelpers.ConvertMessageToBytes(message);
            foreach (IPEndPoint client in Clients) {
                _client.SendTo(messageBytes, client);
            }
        }

        public UdpServerCommunicator(int listeningPort,
                                     SynchronizedCollection<IPEndPoint> clients) : base(listeningPort) {

            Clients = clients;
        }
    }
}