using System.Collections.Generic;
using System.Net;
using Assets.Scripts.Utils;
using Assets.Scripts.Utils.Messages;
using Google.Protobuf;

namespace Assets.Scripts.Network.Server {
    public class TcpServerCommunicatorSenderThread : BaseThread {
        private readonly TcpServerManager _tcpServerManager;

        public TcpServerCommunicatorSenderThread(TcpServerManager tcpServerManager) {
            _tcpServerManager = tcpServerManager;
        }

        protected override void RunThread() {
            while (ThreadShouldRun) {
                var message = _tcpServerManager.Communicator.GetMessageToSend();
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
            var messageBytes = MessagesHelpers.ConvertMessageToBytes(message);
            foreach (var client in _tcpServerManager.Clients) {
                if (clients.Contains(client.GetEndpoint())) {
                    client.Send(messageBytes);
                }
            }
        }

        public void SendToAllClients(IMessage message) {
            var messageBytes = MessagesHelpers.ConvertMessageToBytes(message);
            foreach (var client in _tcpServerManager.Clients) {
                client.Send(messageBytes);
            }
        }
    }
}