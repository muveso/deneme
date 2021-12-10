using System.Collections.Generic;
using System.Net;
using Assets.Scripts.General;
using Assets.Scripts.Network.Client;
using Assets.Scripts.Network.Common;
using Assets.Scripts.Utils.Network.TCP;
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
            Players = new List<ClientDetails>();
            ClientGlobals.Nickname = "PanCHocK";
            GameManager.Instance.IsHost = false;
        }

        private void Update() {
            if (GameManager.Instance.NetworkManagers.ReliableClientManager == null) {
                return;
            }

            if (HandleCommunicatorMessage()) {
                Utils.UI.General.DestroyAllChildren(transform);
                ScrollView.FillScrollViewWithObjects(Players, transform);
            }
        }

        private bool HandleCommunicatorMessage() {
            var message = GameManager.Instance.NetworkManagers.ReliableClientManager.Receive();
            if (message == null) {
                return false;
            }

            if (message.AnyMessage.Is(ServerDisconnectMessage.Descriptor)) {
                SceneManager.LoadScene("MainMenu");
            } else if (message.AnyMessage.Is(
                MainMenuStateMessage.Descriptor)) {
                var mainMenuStateMessage = message.AnyMessage.Unpack<MainMenuStateMessage>();
                UpdateClients(mainMenuStateMessage);
            } else if (message.AnyMessage.Is(StartGameMessage.Descriptor)) {
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
            GameManager.Instance.ServerIpAddress = IPAddress.Parse(IPInputField.text);
            var endpoint = new IPEndPoint(GameManager.Instance.ServerIpAddress, int.Parse(PortInputField.text));
            var tcpMessageBasedClient = new TcpClientMessageBasedClient(new TcpClient(endpoint));
            GameManager.Instance.NetworkManagers.ReliableClientManager =
                new NetworkClientManager(tcpMessageBasedClient);
            ClientMessages.SendClientDetails(GameManager.Instance.NetworkManagers.ReliableClientManager);
            EventSystem.current.currentSelectedGameObject.GetComponent<Button>().interactable = false;
        }

        public void OnClickReady() {
            ClientMessages.SendClientReady(GameManager.Instance.NetworkManagers.ReliableClientManager);
        }
    }
}