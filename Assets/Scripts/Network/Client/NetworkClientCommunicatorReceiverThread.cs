using Assets.Scripts.Utils;

namespace Assets.Scripts.Network.Client {
    public class NetworkClientCommunicatorReceiverThread : BaseThread {
        private readonly NetworkClientCommunicator _networkClientCommunicator;

        public NetworkClientCommunicatorReceiverThread(NetworkClientCommunicator networkClientCommunicator) {
            _networkClientCommunicator = networkClientCommunicator;
        }

        protected override void RunThread() {
            while (ThreadShouldRun) {
                var message = _networkClientCommunicator.NetworkClient.Receive(false);
                if (message != null) {
                    _networkClientCommunicator.AddMessageToReceive(message);
                }
            }
        }
    }
}