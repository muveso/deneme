using System.Collections.Generic;
using System.Net;
using Assets.Scripts.General;
using Assets.Scripts.Network.Client;
using Assets.Scripts.Network.Common;
using Assets.Scripts.Utils.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts.MainMenu {
    public class ClientLobby : MonoBehaviour {
        public InputField IPInputField;
        protected List<ClientDetails> Players;
        public InputField PortInputField;

        private void Awake() {
            NetworkManager.Instance.UpdatePollTimeoutToRefreshRate();
            Players = new List<ClientDetails>();
            ClientGlobals.Nickname = "PanCHocK";
            NetworkManager.Instance.IsHost = false;
        }

        private void Update() {
            if (NetworkManager.Instance.Communicators.ReliableClientCommunicator == null) {
                return;
            }

            if (HandleCommunicatorMessage()) {
                Utils.UI.General.DestroyAllChildren(transform);
                ScrollView.FillScrollViewWithObjects(Players, transform);
            }
        }

        private bool HandleCommunicatorMessage() {
            var message = NetworkManager.Instance.Communicators.ReliableClientCommunicator.Receive();
            if (message == null) {
                return false;
            }

            if (message.ProtobufMessage.Is(MainMenuStateMessage.Descriptor)) {
                var mainMenuStateMessage = message.ProtobufMessage.Unpack<MainMenuStateMessage>();
                UpdateClients(mainMenuStateMessage);
            } else if (message.ProtobufMessage.Is(StartGameMessage.Descriptor)) {
                SceneManager.LoadScene("Game");
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
            NetworkManager.Instance.Communicators.ReliableClientCommunicator =
                new TcpClientCommunicator(new IPEndPoint(IPAddress.Parse(IPInputField.text),
                    int.Parse(PortInputField.text)));
            ClientMessages.SendClientDetails(NetworkManager.Instance.Communicators.ReliableClientCommunicator);
            EventSystem.current.currentSelectedGameObject.GetComponent<Button>().interactable = false;
        }

        public void OnClickReady() {
            ClientMessages.SendClientReady(NetworkManager.Instance.Communicators.ReliableClientCommunicator);
        }
    }
}