using System.Collections.Generic;
using System.Net;
using Evade.Communicators;
using UnityEngine;

namespace Evade.MainMenu {
    public class ClientManager : AbstractClientManager {
        protected TcpClientCommunicator TcpClientCommunicator;

        protected override void Awake() {
            base.Awake();
            ClientGlobals.Nickname = "PanCHocK";
        }

        protected override void Update() {
            base.Update();
        }

        public void OnDestroy() {
            TcpClientCommunicator?.Dispose();
        }

        protected override bool HandleCommunicatorMessage() {
            if (TcpClientCommunicator == null) {
                return false;
            }

            if (!TcpClientCommunicator.MessagesQueue.TryDequeue(out var message)) {
                return false;
            }

            if (message.ProtobufMessage.Is(MainMenuStateMessage.Descriptor)) {
                var mainMenuStateMessage = message.ProtobufMessage.Unpack<MainMenuStateMessage>();
                UpdateClients(mainMenuStateMessage);
            }

            return true;
        }

        private void UpdateClients(MainMenuStateMessage mainMenuStateMessage) {
            var newClientsList = new List<ClientDetails>();
            foreach (var clientDetailsMessage in mainMenuStateMessage.ClientsDetails) {
                newClientsList.Add(new ClientDetails(clientDetailsMessage.Nickname, clientDetailsMessage.IsReady));
            }

            Clients = newClientsList;
        }

        private void SendClientDetails() {
            if (TcpClientCommunicator == null) {
                Debug.LogError("ClientCommunicator is null");
                return;
            }

            var clientDetailsMessage = new ClientDetailsMessage {
                Nickname = ClientGlobals.Nickname
            };
            TcpClientCommunicator.Send(clientDetailsMessage);
        }

        private void InitializeCommunicator() {
            TcpClientCommunicator = new TcpClientCommunicator(IPInputField.text, int.Parse(PortInputField.text));
            ClientGlobals.ServerEndpoint =
                new IPEndPoint(IPAddress.Parse(IPInputField.text), int.Parse(PortInputField.text));
        }

        protected override void ConnectLogic() {
            InitializeCommunicator();
            SendClientDetails();
        }

        public override void OnClickReady() {
            if (TcpClientCommunicator == null) {
                Debug.LogError("ClientCommunicator is null");
                return;
            }

            var clientReadyMessage = new ClientReadyMessage();
            TcpClientCommunicator.Send(clientReadyMessage);
        }
    }
}
