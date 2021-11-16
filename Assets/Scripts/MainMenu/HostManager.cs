using System;
using System.Collections.Generic;
using Evade.Communicators;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Evade.MainMenu {
    public class HostManager : ClientManager {
        private TcpServerCommunicator _tcpServerCommunicator;
        private TcpServerMainMenuProcessingThread _tcpServerMainMenuProcessingThread;

        protected override void Awake() {
            IPInputField.text = GameConsts.DefaultServerIpAddress;
            ClientGlobals.Nickname = "PanCHocKHost";
        }

        public override void OnDestroy() {
            base.OnDestroy();
            _tcpServerMainMenuProcessingThread?.Stop();
            _tcpServerCommunicator?.Dispose();
        }

        protected override void InitializeCommunicator() {
            ClientCommunicator = new HostClientCommunicator(_tcpServerCommunicator, ClientGlobals.Nickname);
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

        protected override List<ClientDetails> GetPlayers() {
            return _tcpServerCommunicator.GetClientDetailsList();
        }

        public void OnClickStartGame() {
            if (AreAllPlayersReady()) {
                Debug.Log("Starting Game");
                GameGlobals.IsHost = true;
                SceneManager.LoadScene("Game");
            } else {
                Debug.Log("Not all clients ready");
            }
        }

        private bool AreAllPlayersReady() {
            foreach (var players in GetPlayers()) {
                if (!players.IsReady) {
                    return false;
                }
            }

            return true;
        }
    }
}