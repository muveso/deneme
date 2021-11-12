using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Evade.MainMenu {
    public class HostManager : ClientManager {
        private const string DEFAULT_SERVER_IP_ADDRESS = "0.0.0.0";
        private const string LOCAL_HOST_IP_ADDRESS = "127.0.0.1";

        private void Start() {
            IPInputField.text = DEFAULT_SERVER_IP_ADDRESS;
            _nickname = "PanCHocK2";
        }

        protected override void InitializeCommunicator() {
            GameManager.Instance.Communicators.TcpClientCommunicator =
                new TcpClientCommunicator(LOCAL_HOST_IP_ADDRESS, int.Parse(PortInputField.text));
            GameManager.Instance.Communicators.TcpClientCommunicator.Start();
        }

        public override void OnClickConnect() {
            try {
                Debug.Log("Starting Server");
                GameManager.Instance.Communicators.TcpServerCommunicator =
                    new TcpServerCommunicator(IPInputField.text, int.Parse(PortInputField.text));
                GameManager.Instance.Communicators.TcpServerCommunicator.Start();

                base.OnClickConnect();
            } catch (Exception e) {
                Debug.LogError(e.Message);
            }
        }

        public void OnClickStartGame() {
            if (AreAllClientsReady()) {
                Debug.Log("Starting Game");
                GameManager.Instance.IsHost = true;
                SceneManager.LoadScene("Game");
            } else {
                Debug.Log("Not all clients ready");
            }
        }

        private bool AreAllClientsReady() {
            if (GameManager.Instance.Communicators.TcpClientCommunicator == null) {
                Debug.LogError("ClientCommunicator is null");
                return false;
            }

            foreach (var clientDetails in GameManager.Instance.Communicators.TcpClientCommunicator.Clients) {
                if (!clientDetails.IsReady) {
                    return false;
                }
            }

            return true;
        }
    }
}