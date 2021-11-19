using System;
using System.Net;
using System.Net.Sockets;
using JetBrains.Annotations;

namespace Assets.Scripts.Utils.Network.UDP {
    public class UdpClient : IDisposable {
        private const int UdpBufferSize = 0x10000;

        public UdpClient(IPEndPoint endpointToConnectTo) {
            Sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            Sock.Connect(endpointToConnectTo);
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

        public void SendAsync(byte[] bytes) {
            Sock.SendAsync(bytes, SocketFlags.None);
        }

        public void SendTo(byte[] bytes, IPEndPoint endpoint) {
            Sock.SendTo(bytes, SocketFlags.None, endpoint);
        }

        public byte[] Receive([NotNull] ref IPEndPoint remoteEp) {
            if (remoteEp == null) throw new ArgumentNullException(nameof(remoteEp));
            EndPoint tempRemoteEp = new IPEndPoint(IPAddress.Any, 0);

            var message = new byte[UdpBufferSize];
            var received = Sock.ReceiveFrom(message, UdpBufferSize, SocketFlags.None, ref tempRemoteEp);
            remoteEp = (IPEndPoint) tempRemoteEp;
            if (received < UdpBufferSize) {
                Array.Resize(ref message, received);
            }

            return message;
        }

        public void Close() {
            Sock.Close();
        }

        public override string ToString() {
            var remoteIpEndPoint = Sock.RemoteEndPoint as IPEndPoint;
            return $"(UdpClient) IP: {remoteIpEndPoint?.Address} | Port: {remoteIpEndPoint?.Port}";
        }
    }
}