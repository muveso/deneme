using System.Net;
using System.Net.Sockets;
using Assets.Scripts.Utils;
using Assets.Scripts.Utils.Messages;
using Google.Protobuf;
using UdpClient = Assets.Scripts.Utils.Network.UDP.UdpClient;

namespace Assets.Scripts.Network.Client {
    public class UdpClientMessageBasedClient : IMessageBasedClient {
        private readonly UdpClient _udpClient;

        public UdpClientMessageBasedClient(UdpClient udpClient) {
            _udpClient = udpClient;
        }

        public void Send(IMessage message) {
            _udpClient.Send(MessagesHelpers.ConvertMessageToBytes(message));
        }

        public MessageToReceive Receive(bool block = true) {
            if (!block && !_udpClient.Sock.Poll(0, SelectMode.SelectRead)) {
                return null;
            }

            var endpoint = new IPEndPoint(IPAddress.Any, 0);
            var message = MessagesHelpers.ConvertBytesToMessage(_udpClient.Receive(ref endpoint));
            return new MessageToReceive(endpoint, message);
        }

        public void Dispose() {
            _udpClient?.Dispose();
        }

        public void SendTo(IMessage message, IPEndPoint endpoint) {
            _udpClient.SendTo(MessagesHelpers.ConvertMessageToBytes(message), endpoint);
        }
    }
}