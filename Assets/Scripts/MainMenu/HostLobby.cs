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
            NetworkManager.Instance.IsHost = true;
        }

        private void OnDestroy() {
            _tcpServerMainMenuProcessingThread?.Stop();
        }

        private void Update() {
            if (NetworkManager.Instance.Communicators.TcpServerCommunicator == null) {
                return;
            }

            if (!_tcpServerMainMenuProcessingThread.StateChanged.WaitOne(0)) {
                return;
            }

            Utils.UI.General.DestroyAllChildren(transform);
            ScrollView.FillScrollViewWithObjects(NetworkManager.Instance.Communicators.TcpServerCommunicator.Clients,
                transform);
        }

        private bool AreAllPlayersReady() {
            foreach (var client in NetworkManager.Instance.Communicators.TcpServerCommunicator
                .Clients) {
                if (!client.Details.IsReady) {
                    return false;
                }
            }

            return true;
        }

        private void InitializeCommunicatorAndServer() {
            // Initialize TCP server
            NetworkManager.Instance.Communicators.TcpServerCommunicator =
                new TcpServerCommunicator(new IPEndPoint(IPAddress.Parse(IPInputField.text),
                    int.Parse(PortInputField.text)));
            // Initialize Processing Thread
            _tcpServerMainMenuProcessingThread =
                new TcpServerMainMenuProcessingThread(NetworkManager.Instance.Communicators.TcpServerCommunicator);
            _tcpServerMainMenuProcessingThread.Start();
            // Initialize Host communicator
            NetworkManager.Instance.Communicators.ReliableClientManager =
                new HostClientManager(NetworkManager.Instance.Communicators.TcpServerCommunicator,
                    ClientGlobals.Nickname);
        }


        public void OnClickStartGame() {
            if (AreAllPlayersReady()) {
                Debug.Log("Starting Game");
                NetworkManager.Instance.Communicators.TcpServerCommunicator.SendAll(new StartGameMessage());
                SceneManager.LoadScene("Game");
            } else {
                Debug.Log("Not all clients ready");
            }
        }

        public void OnClickConnect() {
            InitializeCommunicatorAndServer();
            ClientMessages.SendClientDetails(NetworkManager.Instance.Communicators.ReliableClientManager);
            EventSystem.current.currentSelectedGameObject.GetComponent<Button>().interactable = false;
        }

        public void OnClickReady() {
            ClientMessages.SendClientReady(NetworkManager.Instance.Communicators.ReliableClientManager);
        }
    }
}