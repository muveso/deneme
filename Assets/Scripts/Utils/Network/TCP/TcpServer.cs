using System;
using System.Net;
using System.Net.Sockets;

namespace Evade.Utils.Network{
    public class TcpServer : IDisposable {
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

        public void Close() {
            Sock.Close();
        }

        public void Dispose() {
            Sock.Dispose();
        }
    }
}