using Evade.Communicators;
using UnityEngine;

namespace Evade.Game {
    public class ClientManager : MonoBehaviour {
        protected UdpClientCommunicator UdpClientCommunicator;

        protected virtual void Awake() {
            UdpClientCommunicator = new UdpClientCommunicator(ClientGlobals.ServerEndpoint.Address.ToString(), 5555);
            Debug.Log("Ok");
        }

        protected virtual void OnDestroy() {
            UdpClientCommunicator?.Dispose();
        }

        protected virtual void Update() {
            // Messages from server
            var message = UdpClientCommunicator.TryGetMessageFromQueue();
            if (message != null) { }
        }
    }
}