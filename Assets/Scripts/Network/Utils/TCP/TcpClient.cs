using System;
using System.Net;
using System.Net.Sockets;

public class TcpClient {
    private Socket _sock;

    public TcpClient(IPEndPoint endpoint) {
        _sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        _sock.Connect(endpoint);
    }

    public TcpClient(Socket socket) {
        _sock = socket;
    }

    public void Send(byte[] bytes) {
        if (!_sock.Connected) {
            throw new SocketNotConnectedException();
        }
        byte[] messageLength = BitConverter.GetBytes(bytes.Length);
        _sock.Send(messageLength);
        _sock.Send(bytes);
    }

    public byte[] Recieve(int bytesNum) {
        if (!_sock.Connected) {
            throw new SocketNotConnectedException();
        }
        byte[] bytes = new byte[sizeof(int)];
        _sock.Receive(bytes, sizeof(int), SocketFlags.None);
        int messageLength = BitConverter.ToInt32(bytes);

        byte[] message = new byte[messageLength];
        _sock.Receive(message, messageLength, SocketFlags.None);
        return message;
    }

    public void Close() {
        _sock.Close();
    }
}
