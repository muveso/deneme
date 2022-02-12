using System.Net;
using System.Threading;
using Assets.Scripts.Network.Common;
using Assets.Scripts.Network.Server;
using Assets.Scripts.Utils;
using Assets.Scripts.Utils.Messages;
using Assets.Scripts.Utils.Network;
using UnityEngine;

namespace Assets.Scripts.MainMenu {
    public class TcpServerMainMenuProcessingThread : BaseThread {
        private readonly TcpServerManager _tcpServerManager;

        public TcpServerMainMenuProcessingThread(TcpServerManager tcpServerManager) {
            _tcpServerManager = tcpServerManager;
            StateChanged = new AutoResetEvent(false);
        }

        public AutoResetEvent StateChanged { get; }

        protected override void RunThread() {
            while (ThreadShouldRun) {
                var message = _tcpServerManager.Communicator.Receive();
                if (message != null) {
                    HandleMessage(message);
                    StateChanged.Set();
                }
            }
        }

        private void HandleMessage(MessageToReceive messageToReceive) {
            var protobufMessage = messageToReceive.AnyMessage;
            var client = FindClientByIPEndpoint(messageToReceive.IPEndpoint);
            try {
                if (protobufMessage.Is(ClientReadyMessage.Descriptor)) {
                    Debug.Log($"Client {client.Details.Nickname} sent Ready messageToReceive");
                    client.Details.ToggleReady();
                } else if (protobufMessage.Is(ClientDetailsMessage.Descriptor)) {
                    Debug.Log($"Client {client.Details.Nickname} sent initilize details messageToReceive");
                    var details = protobufMessage.Unpack<ClientDetailsMessage>();
                    client.Details.Nickname = details.Nickname;
                }
            } catch (SocketClosedException) {
                _tcpServerManager.Clients.Remove(client);
            }

            StateUpdateToAllClients();
        }

        private Client FindClientByIPEndpoint(IPEndPoint messageIpEndpoint) {
            foreach (var client in _tcpServerManager.Clients) {
                if (Equals(client.GetEndpoint(), messageIpEndpoint)) {
                    return client;
                }
            }

            return null;
        }

        private void StateUpdateToAllClients() {
            var mainMenuMessage = new MainMenuStateMessage();
            foreach (var client in _tcpServerManager.Clients) {
                var detailsMessage = new ClientDetailsMessage {
                    Nickname = client.Details.Nickname,
                    IsReady = client.Details.IsReady
                };
                mainMenuMessage.ClientsDetails.Add(detailsMessage);
            }

            _tcpServerManager.Communicator.Send(mainMenuMessage);
        }
    }
}