using System.Net;
using System.Net.Sockets;
using TcpClient = Evade.Utils.Network.TcpClient;

namespace Evade.Communicators {
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

        public override string ToString() {
            return $"IP: {TcpClient} | {Details}";
        }
    }

    public class HostClient : Client {
        public HostClient(ClientDetails clientDetails) : base(clientDetails) { }

        public static IPEndPoint GetHostClientEndpoint() {
            return new IPEndPoint(IPAddress.Any, 0);
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

        public override string ToString() {
            return $"Host: | {Details}";
        }
    }
}