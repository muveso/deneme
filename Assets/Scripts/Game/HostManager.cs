using Evade.Communicators;
using UnityEngine;

namespace Evade.Game {
    public class HostManager : ClientManager {
        private UdpServerCommunicator _udpServerCommunicator;

        protected override void Start() {
            _udpServerCommunicator = new UdpServerCommunicator(5555);
            base.Start();
        }

        protected override void OnDestroy() {
            _udpServerCommunicator?.Dispose();
            base.OnDestroy();
        }

        protected override void Update() {
            // All the game logic is here
            // Host does not need to get messages from server because he is the server
            if (_udpServerCommunicator == null) {
                Debug.Log("UdpServerCommunicator is null");
                return;
            }

            base.Update();
            var message = _udpServerCommunicator.TryGetMessageFromQueue();
            if (message != null) {
                Debug.Log("UdpServerCommunicator is null");
            }
        }
    }
}