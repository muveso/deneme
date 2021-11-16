using Assets.Scripts.Network.Common;
using Assets.Scripts.Network.Server;
using Assets.Scripts.Utils;
using Google.Protobuf;

namespace Assets.Scripts.Network.Host {
    public class HostClientCommunicator : IClientCommunicator {
        private readonly TcpServerCommunicator _tcpServerCommunicator;

        public HostClientCommunicator(TcpServerCommunicator tcpServerCommunicator, string nickname) {
            _tcpServerCommunicator = tcpServerCommunicator;
            _tcpServerCommunicator.HostConnect(nickname);
        }

        public void Send(IMessage message) {
            _tcpServerCommunicator.SendMessageFromHost(message);
        }

        public Message GetMessage() {
            return null;
        }

        public void Dispose() { }
    }
}