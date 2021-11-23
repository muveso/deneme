using System.Net;
using Assets.Scripts.Utils;
using UnityEngine;

namespace Assets.Scripts.Network.Server {
    public class UdpServerCommunicatorReceiverThread : BaseThread {
        private readonly UdpServerCommunicator _udpServerCommunicator;

        public UdpServerCommunicatorReceiverThread(UdpServerCommunicator udpCommunicator) {
            _udpServerCommunicator = udpCommunicator;
        }

        protected override void RunThread() {
            while (ThreadShouldRun) {
                var message = _udpServerCommunicator.Client.Receive(false);
                if (message != null) {
                    AddClientIfNotExists(message.IPEndpoint);
                    Debug.Log("Got message, inserting to queue");
                    _udpServerCommunicator.AddMessageToReceive(message);
                }
            }
        }

        public void AddClientIfNotExists(IPEndPoint endpoint) {
            if (!_udpServerCommunicator.Clients.Contains(endpoint)) {
                _udpServerCommunicator.Clients.Add(endpoint);
            }
        }
    }
}