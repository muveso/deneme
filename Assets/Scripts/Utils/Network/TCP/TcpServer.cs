using System;
using System.Net;
using System.Net.Sockets;

namespace Assets.Scripts.Utils.Network.TCP {
    public class TcpServer : IDisposable {
        public TcpServer(string ipAddress, int listeningPort) {
            Sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            var localEndPoint = new IPEndPoint(IPAddress.Parse(ipAddress), listeningPort);
            Sock.Bind(localEndPoint);
            Sock.Listen(10);
        }

        public Socket Sock { get; }

        public void Dispose() {
            Sock.Dispose();
        }

        public TcpClient Accept() {
            return new TcpClient(Sock.Accept());
        }

        public void Close() {
            Sock.Close();
        }
    }
}