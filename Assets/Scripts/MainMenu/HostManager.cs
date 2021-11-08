using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Evade.MainMenu {
    public class HostManager : ClientManager {
        const string DEFAULT_SERVER_IP_ADDRESS = "0.0.0.0";
        const string LOCAL_HOST_IP_ADDRESS = "127.0.0.1";

        private void Start() {
            IPInputField.text = DEFAULT_SERVER_IP_ADDRESS;
            _nickname = "PanCHocK2";
        }

        protected override void InitializeCommunicator() {
            GameManager.Instance.TcpClientCommunicator = new TcpClientCommunicator(LOCAL_HOST_IP_ADDRESS, int.Parse(PortInputField.text));
            GameManager.Instance.TcpClientCommunicator.Start();
        }

        public override void OnClickConnect() {
            try {
                Debug.Log("Starting Server");
                GameManager a = GameManager.Instance;
                GameManager.Instance.TcpServerCommunicator = new TcpServerCommunicator(IPInputField.text, Int32.Parse(PortInputField.text));
                GameManager.Instance.TcpServerCommunicator.Start();

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
            foreach (ClientDetails clientDetails in GameManager.Instance.TcpClientCommunicator.Clients) {
                if (!clientDetails.IsReady) {
                    return false;
                }
            }
            return true;
        }
    }
}