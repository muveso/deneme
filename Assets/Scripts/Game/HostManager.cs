using System.Collections.Generic;
using System.Net;
using Google.Protobuf.WellKnownTypes;

namespace Evade.Game {
    public class HostManager : ClientManager {
        protected override void Start() {
            List<IPEndPoint> clients = GameManager.Instance.TcpServerCommunicator.GetEndpointListFromClients();
            GameManager.Instance.UdpServerCommunicator = new UdpServerCommunicator(5555, clients);
            GameManager.Instance.UdpServerCommunicator.Start();
            base.Start();
        }

        void Update() {
            // All the game logic is here
            // Host does not need to get messages from server because he is the server
            Any message = GameManager.Instance.UdpServerCommunicator.TryGetMessageFromQueue();
            if (message != null) {
            }
        }
    }
}
