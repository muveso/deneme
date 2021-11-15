using System;
using Evade.Communicators;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Evade.MainMenu {
    public class HostManager : ClientManager {
        private const string DefaultServerIpAddress = "0.0.0.0";
        private const string LocalHostIpAddress = "127.0.0.1";
        private TcpServerCommunicator _tcpServerCommunicator;
        private TcpServerMainMenuProcessingThread _tcpServerMainMenuProcessingThread;

        protected override void Awake() {
            IPInputField.text = DefaultServerIpAddress;
            Nickname = "PanCHocK2";
            base.Awake();
        }

        public override void OnDestroy() {
            base.OnDestroy();
            _tcpServerMainMenuProcessingThread?.Stop();
            _tcpServerCommunicator?.Dispose();
        }

        protected override void InitializeCommunicator() {
            TcpClientCommunicator = new TcpClientCommunicator(LocalHostIpAddress, int.Parse(PortInputField.text));
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