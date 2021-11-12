using UnityEngine;

namespace Evade.Game {
    public class HostManager : ClientManager {
        protected override void Start() {
            var clients = GameManager.Instance.Communicators.TcpServerCommunicator.GetEndpointListFromClients();
            GameManager.Instance.Communicators.UdpServerCommunicator = new UdpServerCommunicator(5555, clients);
            GameManager.Instance.Communicators.UdpServerCommunicator.Start();
            base.Start();
        }

        private void Update() {
            // All the game logic is here
            // Host does not need to get messages from server because he is the server
            if (GameManager.Instance.Communicators.UdpServerCommunicator == null) {
                Debug.Log("UdpServerCommunicator is null");
                return;
            }

            var message = GameManager.Instance.Communicators.UdpServerCommunicator.TryGetMessageFromQueue();
            if (message != null) { }
        }
    }
}