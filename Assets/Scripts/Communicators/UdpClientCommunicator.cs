namespace Evade.Communicators {
    public class UdpClientCommunicator : AbstractUdpClientCommunicator {
        private readonly UdpClientReceiverThread _udpClientReceiverThread;

        public UdpClientCommunicator(string ipAddress, int port) : base(ipAddress, port) {
            _udpClientReceiverThread = new UdpClientReceiverThread(this);
            _udpClientReceiverThread.Start();
        }

        public override void Dispose() {
            _udpClientReceiverThread?.Stop();
            base.Dispose();
        }
    }
}