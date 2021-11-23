using Assets.Scripts.Utils;

namespace Assets.Scripts.Network.Client {
    public class NetworkClientCommunicatorSenderThread : BaseThread {
        private readonly NetworkClientCommunicator _networkClientCommunicator;

        public NetworkClientCommunicatorSenderThread(NetworkClientCommunicator networkClientCommunicator) {
            _networkClientCommunicator = networkClientCommunicator;
        }

        protected override void RunThread() {
            while (ThreadShouldRun) {
                var message = _networkClientCommunicator.GetMessageToSend();
                if (message != null) {
                    _networkClientCommunicator.NetworkClient.Send(message);
                }
            }
        }
    }
}