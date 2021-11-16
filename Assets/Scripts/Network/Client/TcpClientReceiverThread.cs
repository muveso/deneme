using System.Net;
using System.Net.Sockets;
using Assets.Scripts.Utils;

namespace Assets.Scripts.Network.Client {
    public class TcpClientReceiverThread : BaseThread {
        private const int PollTimeoutMs = 1000;
        private readonly TcpClientCommunicator _tcpClientCommunicator;

        public TcpClientReceiverThread(TcpClientCommunicator TcpClientCommunicator) {
            _tcpClientCommunicator = TcpClientCommunicator;
        }

        protected override void RunThread() {
            while (ThreadShouldRun) {
                if (_tcpClientCommunicator.Client.Sock.Poll(PollTimeoutMs, SelectMode.SelectRead)) {
                    HandleMessage();
                }
            }
        }

        private void HandleMessage() {
            var messageBytes = _tcpClientCommunicator.Client.Receive();
            var message = MessagesHelpers.ConvertBytesToMessage(messageBytes);
            _tcpClientCommunicator.MessagesQueue.Enqueue(
                new Message((IPEndPoint) _tcpClientCommunicator.Client.Sock.RemoteEndPoint, message));
        }
    }
}