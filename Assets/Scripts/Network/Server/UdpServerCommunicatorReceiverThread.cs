using System.Net;
using Assets.Scripts.Utils;
using UnityEngine;

namespace Assets.Scripts.Network.Server {
    public class UdpServerCommunicatorReceiverThread : BaseThread {
        private readonly UdpServerManager _udpServerManager;

        public UdpServerCommunicatorReceiverThread(UdpServerManager udpManager) {
            _udpServerManager = udpManager;
        }

        protected override void RunThread() {
            while (ThreadShouldRun) {
                var message = _udpServerManager.Client.Receive(false);
                if (message != null) {
                    AddClientIfNotExists(message.IPEndpoint);
                    Debug.Log("Got message, inserting to queue");
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