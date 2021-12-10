using System.Collections.Generic;
using System.Net;
using Assets.Scripts.Utils;
using Google.Protobuf;

namespace Assets.Scripts.Network.Server {
    public class UdpServerCommunicatorSenderThread : BaseThread {
        private readonly UdpServerManager _udpServerManager;

        public UdpServerCommunicatorSenderThread(UdpServerManager udpServerManager) {
            _udpServerManager = udpServerManager;
        }

        protected override void RunThread() {
            while (ThreadShouldRun) {
                var message = _udpServerManager.Communicator.GetMessageToSend();
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
            foreach (var client in _udpServerManager.Clients) {
                if (clients.Contains(client)) {
                    _udpServerManager.Client.SendTo(message, client);
                }
            }
        }

        public void SendToAllClients(IMessage message) {
            foreach (var client in _udpServerManager.Clients) {
                _udpServerManager.Client.SendTo(message, client);
            }
        }
    }
}