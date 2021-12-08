using Assets.Scripts.Utils;

namespace Assets.Scripts.Network.Client {
    public class NetworkClientCommunicatorSenderThread : BaseThread {
        private readonly NetworkClientCommunicator _networkClientCommunicator;

        public NetworkClientCommunicatorSenderThread(NetworkClientCommunicator networkClientCommunicator) {
            _networkClientCommunicator = networkClientCommunicator;
        }

        protected override void RunThread() {
            while (ThreadShouldRun) {
                var message = EnumerableUtils.TryDequeue(_networkClientCommunicator.SendMessagesQueue);
                if (message != null) {
                    _networkClientCommunicator.NetworkClient.Send(message);
                }
            }
        }
    }
}