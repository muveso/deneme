using Evade.Communicators;
using UnityEngine;

namespace Evade.Game {
    public class ClientManager : MonoBehaviour {
        protected UdpCommunicator UdpCommunicator;

        protected virtual void Start() {
            UdpCommunicator = new UdpCommunicator(ClientGlobals.ServerEndpoint.Address.ToString(), 5555);
            UdpCommunicator.Start();
        }

        private void Update() {
            if (UdpCommunicator == null) {
                Debug.Log("UdpClientCommunicator is null");
                return;
            }

            // Messages from server
            var message = UdpCommunicator.TryGetMessageFromQueue();
            if (message != null) { }
        }
    }
}