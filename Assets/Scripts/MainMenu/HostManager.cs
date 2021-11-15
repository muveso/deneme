using System;
using Evade.Communicators;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Evade.MainMenu {
    public class HostManager : ClientManager {
        private TcpServerCommunicator _tcpServerCommunicator;
        private TcpServerMainMenuProcessingThread _tcpServerMainMenuProcessingThread;

        protected override void Awake() {
            base.Awake();
            IPInputField.text = GameConsts.DefaultServerIpAddress;
            ClientGlobals.Nickname = "PanCHocKHost";
        }

        public override void OnDestroy() {
            base.OnDestroy();
            _tcpServerMainMenuProcessingThread?.Stop();
            _tcpServerCommunicator?.Dispose();
        }

        protected override void InitializeCommunicator() {
            TcpClientCommunicator =
                new TcpClientCommunicator(GameConsts.LocalHostIpAddress, int.Parse(PortInputField.text));
        }

        public override void OnClickConnect() {
            try {
                Debug.Log("Starting Server");
                _tcpServerCommunicator = new TcpServerCommunicator(IPInputField.text, int.Parse(PortInputField.text));
                _tcpServerMainMenuProcessingThread = new TcpServerMainMenuProcessingThread(_tcpServerCommunicator);
                _tcpServerMainMenuProcessingThread.Start();
                base.OnClickConnect();
            } catch (Exception e) {
                Debug.LogError(e.Message);
            }
        }

        public void OnClickStartGame() {
            if (AreAllClientsReady()) {
                Debug.Log("Starting Game");
                GameGlobals.IsHost = true;
                SceneManager.LoadScene("Game");
            } else {
                Debug.Log("Not all clients ready");
            }
        }

        private bool AreAllClientsReady() {
            if (TcpClientCommunicator == null) {
                Debug.LogError("ClientCommunicator is null");
                return false;
            }

            foreach (var clientDetails in Clients) {
                if (!clientDetails.IsReady) {
                    return false;
                }
            }

            return true;
        }
    }
}