using System.Net;
using Assets.Scripts.General;
using Assets.Scripts.Network.Common;
using Assets.Scripts.Network.Host;
using Assets.Scripts.Network.Server;
using Assets.Scripts.Utils.Messages;
using Assets.Scripts.Utils.Network;
using Assets.Scripts.Utils.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts.MainMenu {
    public class HostLobby : MonoBehaviour {
        public InputField IPInputField;
        public InputField NicknameInputField;
        public InputField PortInputField;

        private void Awake() {
            IPInputField.text = GameConsts.DefaultServerIpAddress;
            GameManager.Instance.IsHost = true;
        }

        private void Update() {
            if (GameManager.Instance.NetworkManagers.TcpServerManager == null) {
                return;
            }

            var message = GameManager.Instance.NetworkManagers.TcpServerManager.Communicator.Receive();
            if (message != null) {
                HandleMessage(message);
                Utils.UI.General.DestroyAllChildren(transform);
                ScrollView.FillScrollViewWithObjects(GameManager.Instance.NetworkManagers.TcpServerManager.Clients,
                    transform);
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
                GameManager.Instance.NetworkManagers.TcpServerManager.Clients.Remove(client);
            }

            StateUpdateToAllClients();
        }
        
        private Client FindClientByIPEndpoint(IPEndPoint messageIpEndpoint) {
            foreach (var client in GameManager.Instance.NetworkManagers.TcpServerManager.Clients) {
                if (Equals(client.GetEndpoint(), messageIpEndpoint)) {
                    return client;
                }
            }

            return null;
        }

        private void StateUpdateToAllClients() {
            var mainMenuMessage = new MainMenuStateMessage();
            foreach (var client in GameManager.Instance.NetworkManagers.TcpServerManager.Clients) {
                var detailsMessage = new ClientDetailsMessage {
                    Nickname = client.Details.Nickname,
                    IsReady = client.Details.IsReady
                };
                mainMenuMessage.ClientsDetails.Add(detailsMessage);
            }

            GameManager.Instance.NetworkManagers.TcpServerManager.Communicator.Send(mainMenuMessage);
        }
        
        private bool AreAllPlayersReady() {
            foreach (var client in GameManager.Instance.NetworkManagers.TcpServerManager
                .Clients) {
                if (!client.Details.IsReady) {
                    return false;
                }
            }

            return true;
        }

        private void InitializeCommunicatorAndServer() {
            // Initialize TCP server
            GameManager.Instance.NetworkManagers.TcpServerManager =
                new TcpServerManager(new IPEndPoint(IPAddress.Parse(IPInputField.text),
                    int.Parse(PortInputField.text)));
            // Initialize Host communicator
            GameManager.Instance.NetworkManagers.ReliableClientManager =
                new HostClientManager(GameManager.Instance.NetworkManagers.TcpServerManager,
                    GameManager.Instance.Nickname);
        }


        public void OnClickStartGame() {
            if (AreAllPlayersReady()) {
                Debug.Log("Starting Game");
                GameManager.Instance.NetworkManagers.TcpServerManager.Communicator.Send(new StartGameMessage());
                SceneManager.LoadScene("Game");
                Destroy(this);
            } else {
                Debug.Log("Not all clients ready");
            }
        }

        public void OnClickConnect() {
            GameManager.Instance.Nickname = NicknameInputField.text;
            InitializeCommunicatorAndServer();
            ClientMessages.SendClientDetails(GameManager.Instance.NetworkManagers.ReliableClientManager);
            EventSystem.current.currentSelectedGameObject.GetComponent<Button>().interactable = false;
        }

        public void OnClickReady() {
            ClientMessages.SendClientReady(GameManager.Instance.NetworkManagers.ReliableClientManager);
        }
    }
}