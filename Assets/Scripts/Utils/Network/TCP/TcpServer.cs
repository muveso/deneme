using System;
using System.Net;
using System.Net.Sockets;

namespace Assets.Scripts.Utils.Network.TCP {
    public class TcpServer : IDisposable {
        public TcpServer(IPEndPoint localEndpoint) {
            Sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Sock.Bind(localEndpoint);
            Sock.Listen(10);
        }

        public Socket Sock { get; }

        public void Dispose() {
            Sock.Dispose();
        }

        public TcpClient Accept() {
            return new TcpClient(Sock.Accept());
        }
    }
}