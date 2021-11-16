using System.Net;
using System.Net.Sockets;
using TcpClient = Assets.Scripts.Utils.Network.TCP.TcpClient;

namespace Assets.Scripts.Network.Common {
    public abstract class Client {
        protected Client() {
            Details = new ClientDetails();
        }

        protected Client(ClientDetails clientDetails) {
            Details = clientDetails;
        }

        public ClientDetails Details { get; }

        public abstract void Send(byte[] bytes);
        public abstract byte[] Receive();
        public abstract IPEndPoint GetEndpoint();
        public abstract Socket GetSock();

        public override string ToString() {
            return $"Nickname: {Details.Nickname} | Ready: {Details.IsReady}";
        }
    }

    public class NetworkTcpClient : Client {
        public NetworkTcpClient(TcpClient tcpClient) {
            TcpClient = tcpClient;
        }

        public TcpClient TcpClient { get; }

        public override void Send(byte[] bytes) {
            TcpClient.Send(bytes);
        }

        public override byte[] Receive() {
            return TcpClient.Receive();
        }

        public override IPEndPoint GetEndpoint() {
            return TcpClient.Sock.RemoteEndPoint as IPEndPoint;
        }

        public override Socket GetSock() {
            return TcpClient.Sock;
        }
    }

    public class HostClient : Client {
        public HostClient(ClientDetails clientDetails) : base(clientDetails) { }

        public static IPEndPoint GetHostClientEndpoint() {
            return new IPEndPoint(IPAddress.Any, 0);
        }

        public static bool IsHostClient(Client other) {
            return Equals(other.GetEndpoint(), GetHostClientEndpoint());
        }

        public override void Send(byte[] bytes) {
            // Do nothing
        }

        public override byte[] Receive() {
            return null;
        }

        public override IPEndPoint GetEndpoint() {
            return GetHostClientEndpoint();
        }

        public override Socket GetSock() {
            return null;
        }
    }
}