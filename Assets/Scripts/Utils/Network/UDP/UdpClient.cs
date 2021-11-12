using System;
using System.Net;
using System.Net.Sockets;

namespace Evade.Utils.Network {
    public class UdpClient : IDisposable {
        private const int UDP_BUFFER_SIZE = 0x10000;

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

        public Socket Sock { get; }

        public bool IsConnected => Sock.Connected;

        public void Dispose() {
            Sock.Dispose();
        }

        public void Send(byte[] bytes) {
            Sock.Send(bytes, SocketFlags.None);
        }

        public void SendTo(byte[] bytes, IPEndPoint endpoint) {
            Sock.SendTo(bytes, SocketFlags.None, endpoint);
        }

        public byte[] Recieve() {
            var message = new byte[UDP_BUFFER_SIZE];
            var received = Sock.Receive(message, UDP_BUFFER_SIZE, SocketFlags.None);
            Array.Resize(ref message, received);
            return message;
        }

        public void Close() {
            Sock.Close();
        }

        public override string ToString() {
            var remoteIpEndPoint = Sock.RemoteEndPoint as IPEndPoint;
            return $"(UdpClient) IP: {remoteIpEndPoint.Address} | Port: {remoteIpEndPoint.Port.ToString()}";
        }
    }
}