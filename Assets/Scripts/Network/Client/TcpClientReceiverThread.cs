using System.Net;
using System.Net.Sockets;
using Assets.Scripts.Utils;

namespace Assets.Scripts.Network.Client {
    public class TcpClientReceiverThread : BaseThread {
        private const int PollTimeoutMs = 1000;
        private readonly TcpClientCommunicator _TcpClientCommunicator;

        public TcpClientReceiverThread(TcpClientCommunicator TcpClientCommunicator) {
            _TcpClientCommunicator = TcpClientCommunicator;
        }

        protected override void RunThread() {
            while (ThreadShouldRun) {
                if (_TcpClientCommunicator.Client.Sock.Poll(PollTimeoutMs, SelectMode.SelectRead)) {
                    HandleMessage();
                }
            }
        }

        private void HandleMessage() {
            var messageBytes = _TcpClientCommunicator.Client.Receive();
            var message = MessagesHelpers.ConvertBytesToMessage(messageBytes);
            _TcpClientCommunicator.MessagesQueue.Enqueue(
                new Message((IPEndPoint) _TcpClientCommunicator.Client.Sock.RemoteEndPoint, message));
        }
    }
}