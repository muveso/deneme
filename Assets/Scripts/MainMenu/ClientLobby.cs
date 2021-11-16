using System.Collections.Generic;
using Assets.Scripts.General;
using Assets.Scripts.Network.Client;
using Assets.Scripts.Network.Common;
using Assets.Scripts.Utils.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts.MainMenu {
    public class ClientLobby : MonoBehaviour {
        public InputField IPInputField;
        protected List<ClientDetails> Players;
        public InputField PortInputField;

        private void Awake() {
            Players = new List<ClientDetails>();
            ClientGlobals.Nickname = "PanCHocK";
            NetworkManager.Instance.IsHost = true;
        }

        private void Update() {
            if (NetworkManager.Instance.Communicators.ClientCommunicator == null) {
                return;
            }

            if (HandleCommunicatorMessage()) {
                Utils.UI.General.DestroyAllChildren(transform);
                ScrollView.FillScrollViewWithObjects(Players, transform);
            }
        }

        private bool HandleCommunicatorMessage() {
            var message = NetworkManager.Instance.Communicators.ClientCommunicator.GetMessage();
            if (message == null) {
                return false;
            }

            if (message.ProtobufMessage.Is(MainMenuStateMessage.Descriptor)) {
                var mainMenuStateMessage = message.ProtobufMessage.Unpack<MainMenuStateMessage>();
                UpdateClients(mainMenuStateMessage);
            }

            return true;
        }

        private void UpdateClients(MainMenuStateMessage mainMenuStateMessage) {
            var newPlayersList = new List<ClientDetails>();
            foreach (var clientDetailsMessage in mainMenuStateMessage.ClientsDetails) {
                newPlayersList.Add(new ClientDetails(clientDetailsMessage.Nickname, clientDetailsMessage.IsReady));
            }

            Players = newPlayersList;
        }

        public void OnClickConnect() {
            NetworkManager.Instance.Communicators.ClientCommunicator =
                new TcpClientCommunicator(IPInputField.text, int.Parse(PortInputField.text));
            ClientMessages.SendClientDetails(NetworkManager.Instance.Communicators.ClientCommunicator);
            EventSystem.current.currentSelectedGameObject.GetComponent<Button>().interactable = false;
        }

        public void OnClickReady() {
            ClientMessages.SendClientReady(NetworkManager.Instance.Communicators.ClientCommunicator);
        }
    }
}