using Assets.Scripts.Utils;

namespace Assets.Scripts.Network.Client {
    public class NetworkClientCommunicatorSenderThread : BaseThread {
        private readonly NetworkClientManager _networkClientManager;

        public NetworkClientCommunicatorSenderThread(NetworkClientManager networkClientManager) {
            _networkClientManager = networkClientManager;
        }

        protected override void RunThread() {
            while (ThreadShouldRun) {
                var message = _networkClientManager.Communicator.GetMessageToSend(1 * 1000);
                if (message != null) {
                    _networkClientManager.NetworkClient.Send(message.Message);
                }
            }
        }
    }
}