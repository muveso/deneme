using System;
using System.Net;
using Evade.Communicators;
using Evade.Utils;
using Google.Protobuf.WellKnownTypes;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Evade.MainMenu {
    public class HostManager : AbstractClientManager {
        private const string DefaultServerIpAddress = "0.0.0.0";
        private TcpServerCommunicator _tcpServerCommunicator;
        private TcpServerMainMenuProcessingThread _tcpServerMainMenuProcessingThread;

        protected override void Awake() {
            base.Awake();
            IPInputField.text = DefaultServerIpAddress;
            ClientGlobals.Nickname = "PanCHocKHost";
        }

        protected override bool HandleCommunicatorMessage() {
            throw new NotImplementedException();
        }

        private void OnDestroy() {
            _tcpServerMainMenuProcessingThread?.Stop();
            _tcpServerCommunicator?.Dispose();
        }

        protected override void ConnectLogic() {
            Debug.Log("Starting Server");
            _tcpServerCommunicator = new TcpServerCommunicator(IPInputField.text, int.Parse(PortInputField.text));
            _tcpServerMainMenuProcessingThread = new TcpServerMainMenuProcessingThread(_tcpServerCommunicator);
            _tcpServerMainMenuProcessingThread.Start();
        }

        public override void OnClickReady() {
            var clientReadyMessage = new ClientReadyMessage();
            _tcpServerCommunicator.MessagesQueue.Enqueue(new Message(new IPEndPoint(IPAddress.Any, 0),
                Any.Pack(clientReadyMessage)));
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
            foreach (var clientDetails in Clients) {
                if (!clientDetails.IsReady) {
                    return false;
                }
            }

            return true;
        }
    }
}
