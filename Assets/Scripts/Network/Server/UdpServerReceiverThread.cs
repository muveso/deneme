using Assets.Scripts.Utils;
using UnityEngine;

namespace Assets.Scripts.Network.Server {
    public class UdpServerReceiverThread : BaseThread {
        private readonly UdpServerCommunicator _udpServerCommunicator;

        public UdpServerReceiverThread(UdpServerCommunicator udpCommunicator) {
            _udpServerCommunicator = udpCommunicator;
        }

        protected override void RunThread() {
            while (ThreadShouldRun) {
                var message = _udpServerCommunicator.Receive(false);
                if (message != null) {
                    _udpServerCommunicator.AddClientIfNotExists(message.IPEndpoint);
                    Debug.Log("Got message, inserting to queue");
                    _udpServerCommunicator.AddMessage(message);
                }
            }
        }
    }
}