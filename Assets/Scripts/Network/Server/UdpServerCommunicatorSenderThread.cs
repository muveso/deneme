using System.Collections.Generic;
using System.Net;
using Assets.Scripts.Utils;
using Google.Protobuf;

namespace Assets.Scripts.Network.Server {
    public class UdpServerCommunicatorSenderThread : BaseThread {
        private readonly UdpServerCommunicator _udpServerCommunicator;

        public UdpServerCommunicatorSenderThread(UdpServerCommunicator udpServerCommunicator) {
            _udpServerCommunicator = udpServerCommunicator;
        }

        protected override void RunThread() {
            while (ThreadShouldRun) {
                var message = _udpServerCommunicator.GetMessageToSend();
                if (message != null) {
                    if (message.IPEndpoints != null) {
                        SendToClients(message.IPEndpoints, message.Message);
                    } else {
                        SendToAllClients(message.Message);
                    }
                }
            }
        }

        public void SendToClients(List<IPEndPoint> clients, IMessage message) {
            foreach (var client in _udpServerCommunicator.Clients) {
                if (clients.Contains(client)) {
                    _udpServerCommunicator.Client.SendTo(message, client);
                }
            }
        }

        public void SendToAllClients(IMessage message) {
            foreach (var client in _udpServerCommunicator.Clients) {
                _udpServerCommunicator.Client.SendTo(message, client);
            }
        }
    }
}