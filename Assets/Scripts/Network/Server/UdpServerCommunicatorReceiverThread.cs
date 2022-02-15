using System.Net;
using Assets.Scripts.Utils;

namespace Assets.Scripts.Network.Server {
    public class UdpServerCommunicatorReceiverThread : BaseThread {
        private readonly UdpServerManager _udpServerManager;

        public UdpServerCommunicatorReceiverThread(UdpServerManager udpManager) {
            _udpServerManager = udpManager;
        }

        protected override void RunThread() {
            while (ThreadShouldRun) {
                var message = _udpServerManager.Client.Receive(1 * 1000, false);
                if (message != null) {
                    AddClientIfNotExists(message.IPEndpoint);
                    _udpServerManager.AddMessageToReceive(message);
                }
            }
        }

        public void AddClientIfNotExists(IPEndPoint endpoint) {
            if (!_udpServerManager.Clients.Contains(endpoint)) {
                _udpServerManager.Clients.Add(endpoint);
            }
        }
    }
}