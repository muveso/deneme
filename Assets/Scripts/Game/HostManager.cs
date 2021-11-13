using Evade.Communicators;
using UnityEngine;

namespace Evade.Game {
    public class HostManager : ClientManager {
        private UdpServerCommunicator _udpServerCommunicator;

        protected override void Start() {
            _udpServerCommunicator = new UdpServerCommunicator(5555);
            _udpServerCommunicator.Start();
            base.Start();
        }

        private void Update() {
            // All the game logic is here
            // Host does not need to get messages from server because he is the server
            if (_udpServerCommunicator == null) {
                Debug.Log("UdpServerCommunicator is null");
                return;
            }

            var message = _udpServerCommunicator.TryGetMessageFromQueue();
            if (message != null) { }
        }
    }
}