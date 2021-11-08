using System;
using System.Net;
using System.Net.Sockets;

namespace Evade.Utils.Network {
    public class UdpClient {
        public Socket Sock { get; private set; }
        const int UDP_BUFFER_SIZE = 0x10000;

        public UdpClient(IPEndPoint endpointToConnectTo) {
            Sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            Sock.Connect(endpointToConnectTo);
        }

        public UdpClient(string ipAddress, int port) : this(new IPEndPoint(IPAddress.Parse(ipAddress), port)) {
            // Blank
        }

        public UdpClient(int listenPort) {
            Sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            Sock.Bind(new IPEndPoint(IPAddress.Any, listenPort));
        }

        public bool IsConnected => Sock.Connected;

        public void Send(byte[] bytes) {
            Sock.Send(bytes, SocketFlags.None);
        }

        public void SendTo(byte[] bytes, IPEndPoint endpoint) {
            Sock.SendTo(bytes, SocketFlags.None, endpoint);
        }

        public byte[] Recieve() {   
            byte[] message = new byte[UDP_BUFFER_SIZE];
            int received = Sock.Receive(message, UDP_BUFFER_SIZE, SocketFlags.None);

            if (received < UDP_BUFFER_SIZE) {
                byte[] newBuffer = new byte[received];
                Buffer.BlockCopy(message, 0, newBuffer, 0, received);
                return newBuffer;
            }
            return message;
        }

        public void Close() {
            Sock.Close();
        }

        public override string ToString() {
            IPEndPoint remoteIpEndPoint = Sock.RemoteEndPoint as IPEndPoint;
            return $"(UdpClient) IP: {remoteIpEndPoint.Address.ToString()} | Port: {remoteIpEndPoint.Port.ToString()}";
        }
    }
}