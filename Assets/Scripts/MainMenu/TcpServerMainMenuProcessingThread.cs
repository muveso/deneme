using System.Net;
using Evade.Utils;
using Evade.Utils.Network;
using UnityEngine;

namespace Evade.Communicators {
    public class TcpServerMainMenuProcessingThread : BaseThread {
        private readonly TcpServerCommunicator _tcpServerCommunicator;

        public TcpServerMainMenuProcessingThread(TcpServerCommunicator tcpServerCommunicator) {
            _tcpServerCommunicator = tcpServerCommunicator;
        }

        protected override void RunThread() {
            while (ThreadShouldRun) {
                if (_tcpServerCommunicator.MessagesQueue.TryDequeue(out var message)) {
                    HandleMessage(message);
                }
            }
        }

        private void HandleMessage(Message message) {
            var protobufMessage = message.ProtobufMessage;
            var client = FindClientByIPEndpoint(message.IPEndpoint);
            try {
                if (protobufMessage.Is(ClientReadyMessage.Descriptor)) {
                    Debug.Log($"Client {client.Details.Nickname} sent Ready message");
                    client.Details.ToggleReady();
                } else if (protobufMessage.Is(ClientDetailsMessage.Descriptor)) {
                    Debug.Log($"Client {client.Details.Nickname} sent initilize details message");
                    var details = protobufMessage.Unpack<ClientDetailsMessage>();
                    client.Details.Nickname = details.Nickname;
                }
            } catch (SocketClosedException) {
                _tcpServerCommunicator.Clients.Remove(client);
            }

            StateUpdateToAllClients();
        }

        private Client FindClientByIPEndpoint(IPEndPoint messageIpEndpoint) {
            foreach (var client in _tcpServerCommunicator.Clients) {
                if (Equals(client.TcpClient.Sock.RemoteEndPoint, messageIpEndpoint)) {
                    return client;
                }
            }

            return null;
        }

        private void StateUpdateToAllClients() {
            var mainMenuMessage = new MainMenuStateMessage();
            foreach (var client in _tcpServerCommunicator.Clients) {
                var detailsMessage = new ClientDetailsMessage {
                    Nickname = client.Details.Nickname,
                    IsReady = client.Details.IsReady
                };
                mainMenuMessage.ClientsDetails.Add(detailsMessage);
            }

            _tcpServerCommunicator.SendToAllClients(mainMenuMessage);
        }
    }
}