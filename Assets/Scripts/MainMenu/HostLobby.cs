using System.Net;
using Assets.Scripts.General;
using Assets.Scripts.Network.Common;
using Assets.Scripts.Network.Host;
using Assets.Scripts.Network.Server;
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
            GameManager.Instance.IsHost = true;
        }

        private void Update() {
            if (GameManager.Instance.NetworkManagers.TcpServerManager == null) {
                return;
            }
            
            Utils.UI.General.DestroyAllChildren(transform);
            ScrollView.FillScrollViewWithObjects(GameManager.Instance.NetworkManagers.TcpServerManager.Clients,
                transform);
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
            ServerLobby serverLobby = gameObject.AddComponent<ServerLobby>();
            serverLobby.StartServer(IPInputField.text, PortInputField.text);
            // Initialize Host communicator
            GameManager.Instance.NetworkManagers.ReliableClientManager =
                new HostClientManager(GameManager.Instance.NetworkManagers.TcpServerManager,
                    GameManager.Instance.Nickname);
        }


        public void OnClickStartGame() {
            TryToStartGame();
        }

        private void TryToStartGame() {
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