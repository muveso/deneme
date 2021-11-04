using System;
using System.Net;
using System.Net.Sockets;

namespace Utils.Network{
    public class TcpServer {
        public Socket Sock { get; private set; }

        public TcpServer(string ipAddress, int listeningPort) {
            Sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Parse(ipAddress), listeningPort);
            Sock.Bind(localEndPoint);
            Sock.Listen(10);
        }

        public TcpClient Accept() {
            return new TcpClient(Sock.Accept());
        }
    }
}