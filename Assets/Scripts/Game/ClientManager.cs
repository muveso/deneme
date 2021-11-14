using Evade.Communicators;
using UnityEngine;

namespace Evade.Game {
    public class ClientManager : MonoBehaviour {
        protected UdpClientCommunicator UdpClientCommunicator;

        protected virtual void Start() {
            UdpClientCommunicator = new UdpClientCommunicator(ClientGlobals.ServerEndpoint.Address.ToString(), 5555);
        }

        protected virtual void OnDestroy() {
            UdpClientCommunicator?.Dispose();
        }

        protected virtual void Update() {
            if (UdpClientCommunicator == null) {
                Debug.Log("UdpClientCommunicator is null");
                return;
            }

            var clientReadyMessage = new ClientReadyMessage();
            UdpClientCommunicator.Send(clientReadyMessage);
            // Messages from server
            var message = UdpClientCommunicator.TryGetMessageFromQueue();
            if (message != null) { }
        }
    }
}