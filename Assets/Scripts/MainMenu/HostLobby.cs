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
        private TcpServerMainMenuProcessingThread _tcpServerMainMenuProcessingThread;
        public InputField IPInputField;
        public InputField PortInputField;

        private void Awake() {
            IPInputField.text = GameConsts.DefaultServerIpAddress;
            ClientGlobals.Nickname = "PanCHocKHost";
            GameManager.Instance.IsHost = true;
        }

        private void OnDestroy() {
            _tcpServerMainMenuProcessingThread?.Stop();
        }

        private void Update() {
            if (GameManager.Instance.NetworkManagers.TcpServerManager == null) {
                return;
            }

            if (!_tcpServerMainMenuProcessingThread.StateChanged.WaitOne(0)) {
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
            // Initialize TCP server
            GameManager.Instance.NetworkManagers.TcpServerManager =
                new TcpServerManager(new IPEndPoint(IPAddress.Parse(IPInputField.text),
                    int.Parse(PortInputField.text)));
            // Initialize Processing Thread
            _tcpServerMainMenuProcessingThread =
                new TcpServerMainMenuProcessingThread(GameManager.Instance.NetworkManagers.TcpServerManager);
            _tcpServerMainMenuProcessingThread.Start();
            // Initialize Host communicator
            GameManager.Instance.NetworkManagers.ReliableClientManager =
                new HostClientClientManager(GameManager.Instance.NetworkManagers.TcpServerManager,
                    ClientGlobals.Nickname);
        }


        public void OnClickStartGame() {
            if (AreAllPlayersReady()) {
                Debug.Log("Starting Game");
                GameManager.Instance.NetworkManagers.TcpServerManager.Communicator.Send(new StartGameMessage());
                SceneManager.LoadScene("Game");
            } else {
                Debug.Log("Not all clients ready");
            }
        }

        public void OnClickConnect() {
            InitializeCommunicatorAndServer();
            ClientMessages.SendClientDetails(GameManager.Instance.NetworkManagers.ReliableClientManager);
            EventSystem.current.currentSelectedGameObject.GetComponent<Button>().interactable = false;
        }

        public void OnClickReady() {
            ClientMessages.SendClientReady(GameManager.Instance.NetworkManagers.ReliableClientManager);
        }
    }
}