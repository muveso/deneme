using System.Net;
using System.Net.Sockets;
using Assets.Scripts.Utils;
using Assets.Scripts.Utils.Messages;
using Google.Protobuf;
using TcpClient = Assets.Scripts.Utils.Network.TCP.TcpClient;

namespace Assets.Scripts.Network.Client {
    public class TcpClientMessageBasedClient : IMessageBasedClient {
        private readonly TcpClient _tcpClient;

        public TcpClientMessageBasedClient(TcpClient client) {
            _tcpClient = client;
        }

        public void Send(IMessage message) {
            _tcpClient.Send(MessagesHelpers.ConvertMessageToBytes(message));
        }

        public MessageToReceive Receive(bool block = true) {
            if (!block && !_tcpClient.Sock.Poll(0, SelectMode.SelectRead)) {
                return null;
            }

            var message = MessagesHelpers.ConvertBytesToMessage(_tcpClient.Receive());
            return new MessageToReceive((IPEndPoint) _tcpClient.Sock.RemoteEndPoint, message);
        }

        public void Dispose() {
            _tcpClient?.Dispose();
        }
    }
}