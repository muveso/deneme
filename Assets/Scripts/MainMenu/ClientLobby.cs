using System.Collections.Generic;
using System.Net;
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
            if (NetworkManager.Instance.Communicators.TcpClientCommunicator == null) {
                return;
            }

            if (HandleCommunicatorMessage()) {
                Utils.UI.General.DestroyAllChildren(transform);
                ScrollView.FillScrollViewWithObjects(Players, transform);
            }
        }

        private bool HandleCommunicatorMessage() {
            var message = NetworkManager.Instance.Communicators.TcpClientCommunicator.GetMessage();
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
            NetworkManager.Instance.Communicators.TcpClientCommunicator =
                new TcpClientCommunicator(new IPEndPoint(IPAddress.Parse(IPInputField.text),
                    int.Parse(PortInputField.text)));
            ClientMessages.SendClientDetails(NetworkManager.Instance.Communicators.TcpClientCommunicator);
            EventSystem.current.currentSelectedGameObject.GetComponent<Button>().interactable = false;
        }

        public void OnClickReady() {
            ClientMessages.SendClientReady(NetworkManager.Instance.Communicators.TcpClientCommunicator);
        }
    }
}