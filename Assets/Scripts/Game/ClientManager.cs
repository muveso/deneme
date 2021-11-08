using Google.Protobuf.WellKnownTypes;
using UnityEngine;

namespace Evade.Game {
    public class ClientManager : MonoBehaviour {
        protected virtual void Start() {
            string remoteAddress = GameManager.Instance.TcpClientCommunicator.GetRemoteEndpoint().Address.ToString();
            GameManager.Instance.UdpClientCommunicator = new UdpCommunicator(remoteAddress, 5555);
            GameManager.Instance.UdpClientCommunicator.Start();
        }
        void Update() {
            // Messages from server
            Any message = GameManager.Instance.UdpClientCommunicator.TryGetMessageFromQueue();
            if (message != null) {
            }
        }
    }
}
