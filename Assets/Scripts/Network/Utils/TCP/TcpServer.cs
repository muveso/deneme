using System;
using System.Net;
using System.Net.Sockets;

public class TcpServer {
    private Socket _sock;

    public TcpServer(int listeningPort) {
        _sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, listeningPort);
        _sock.Bind(localEndPoint);
        _sock.Listen(10);
    }

    public TcpClient Accept() {
        return new TcpClient(_sock.Accept());
    }
}
