using System.Net;
using Assets.Scripts.Network.Common;

namespace Assets.Scripts.Network.Client {
    public class UdpClientCommunicator : AbstractUdpCommunicator {
        private readonly UdpClientReceiverThread _udpClientReceiverThread;

        public UdpClientCommunicator(IPEndPoint endpoint) : base(endpoint) {
            _udpClientReceiverThread = new UdpClientReceiverThread(this);
            _udpClientReceiverThread.Start();
        }

        public override void Dispose() {
            _udpClientReceiverThread?.Stop();
            base.Dispose();
        }
    }
}