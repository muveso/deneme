using System.Collections.Generic;
using System.Net;
using Google.Protobuf.WellKnownTypes;
using UnityEngine;

namespace Evade.Game {
    public class HostManager : ClientManager {
        protected override void Start() {
            List<IPEndPoint> clients = GameManager.Instance.Communicators.TcpServerCommunicator.GetEndpointListFromClients();
            GameManager.Instance.Communicators.UdpServerCommunicator = new UdpServerCommunicator(5555, clients);
            GameManager.Instance.Communicators.UdpServerCommunicator.Start();
            base.Start();
        }

        void Update() {
            // All the game logic is here
            // Host does not need to get messages from server because he is the server
            if (GameManager.Instance.Communicators.UdpServerCommunicator == null) {
                Debug.Log("UdpServerCommunicator is null");
                return;
            }
            Any message = GameManager.Instance.Communicators.UdpServerCommunicator.TryGetMessageFromQueue();
            if (message != null) {
            }
        }
    }
}
