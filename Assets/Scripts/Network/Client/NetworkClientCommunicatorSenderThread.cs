using Assets.Scripts.Utils;

namespace Assets.Scripts.Network.Client {
    public class NetworkClientCommunicatorSenderThread : BaseThread {
        private readonly NetworkClientClientManager _networkClientClientManager;

        public NetworkClientCommunicatorSenderThread(NetworkClientClientManager networkClientClientManager) {
            _networkClientClientManager = networkClientClientManager;
        }

        protected override void RunThread() {
            while (ThreadShouldRun) {
                var message = _networkClientClientManager.Communicator.GetMessageToSend();
                if (message != null) {
                    _networkClientClientManager.NetworkClient.Send(message.Message);
                }
            }
        }
    }
}