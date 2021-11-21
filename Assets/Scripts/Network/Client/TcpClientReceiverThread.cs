using System.Net;
using System.Net.Sockets;
using Assets.Scripts.Utils;

namespace Assets.Scripts.Network.Client {
    public class TcpClientReceiverThread : BaseThread {
        private readonly TcpClientCommunicator _tcpClientCommunicator;

        public TcpClientReceiverThread(TcpClientCommunicator tcpClientCommunicator) {
            _tcpClientCommunicator = tcpClientCommunicator;
        }

        protected override void RunThread() {
            while (ThreadShouldRun) {
                if (_tcpClientCommunicator.Client.Sock.Poll(0, SelectMode.SelectRead)) {
                    var messageBytes = _tcpClientCommunicator.Client.Receive();
                    var message = MessagesHelpers.ConvertBytesToMessage(messageBytes);
                    _tcpClientCommunicator.MessagesQueue.AddMessage(
                        new Message((IPEndPoint) _tcpClientCommunicator.Client.Sock.RemoteEndPoint, message));
                }
            }
        }
    }
}