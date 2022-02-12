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

        private void InitializeCommunicatorAndServer() {
            ServerLobby serverLobby = gameObject.AddComponent<ServerLobby>();
            serverLobby.StartServer(IPInputField.text, PortInputField.text);
            // Initialize Host communicator
            GameManager.Instance.NetworkManagers.ReliableClientManager =
                new HostClientManager(GameManager.Instance.NetworkManagers.TcpServerManager,
                    GameManager.Instance.Nickname);
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