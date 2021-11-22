using Assets.Scripts.Utils;

namespace Assets.Scripts.Network.Client {
    public class NetworkClientReceiverThread : BaseThread {
        private readonly NetworkClientCommunicator _networkClientCommunicator;

        public NetworkClientReceiverThread(NetworkClientCommunicator networkClientCommunicator) {
            _networkClientCommunicator = networkClientCommunicator;
        }

        protected override void RunThread() {
            while (ThreadShouldRun) {
                var message = _networkClientCommunicator.Receive(false);
                if (message != null) {
                    _networkClientCommunicator.AddMessage(message);
                }
            }
        }
    }
}