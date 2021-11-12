using UnityEngine;

namespace Evade.Game {
    public class ClientManager : MonoBehaviour {
        protected virtual void Start() {
            var remoteAddress = GameManager.Instance.Communicators.TcpClientCommunicator.GetRemoteEndpoint().Address
                .ToString();
            GameManager.Instance.Communicators.UdpClientCommunicator = new UdpCommunicator(remoteAddress, 5555);
            GameManager.Instance.Communicators.UdpClientCommunicator.Start();
        }

        private void Update() {
            if (GameManager.Instance.Communicators.UdpClientCommunicator == null) {
                Debug.Log("UdpClientCommunicator is null");
                return;
            }

            // Messages from server
            var message = GameManager.Instance.Communicators.UdpClientCommunicator.TryGetMessageFromQueue();
            if (message != null) { }
        }
    }
}