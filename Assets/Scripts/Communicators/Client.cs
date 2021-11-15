using Evade.Utils.Network;

namespace Evade.Communicators {
    public class Client {
        public Client(TcpClient tcpClient) {
            TcpClient = tcpClient;
            Details = new ClientDetails();
        }

        public ClientDetails Details { get; }
        public TcpClient TcpClient { get; }

        public override string ToString() {
            return $"IP: {TcpClient} | {Details}";
        }
    }
}