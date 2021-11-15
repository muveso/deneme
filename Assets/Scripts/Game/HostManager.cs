using Evade.Communicators;
using UnityEngine;

namespace Evade.Game {
    public class HostManager : ClientManager {
        private UdpServerCommunicator _udpServerCommunicator;

        protected override void Awake() {
            _udpServerCommunicator = new UdpServerCommunicator(ClientGlobals.ServerEndpoint.Port);
            base.Awake();
        }

        protected override void OnDestroy() {
            _udpServerCommunicator?.Dispose();
            base.OnDestroy();
        }

        protected override void Update() {
            // All the game logic is here
            // Host does not need to get messages from server because he is the server

            var message = _udpServerCommunicator.TryGetMessageFromQueue();
            if (message != null) {
                Debug.Log("HostManager got message");
            }
        }
    }
}