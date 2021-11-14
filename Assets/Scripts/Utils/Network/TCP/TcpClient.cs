using System;
using System.Net;
using System.Net.Sockets;

namespace Evade.Utils.Network {
    public class TcpClient : IDisposable {
        public TcpClient(IPEndPoint endpoint) {
            Sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Sock.Connect(endpoint);
        }

        public TcpClient(Socket socket) {
            Sock = socket;
        }

        public Socket Sock { get; }

        public bool IsConnected => Sock.Connected;

        public void Dispose() {
            Sock.Dispose();
        }

        public void Send(byte[] bytes) {
            if (!Sock.Connected) {
                throw new SocketNotConnectedException();
            }

            var messageLength = BitConverter.GetBytes(bytes.Length);
            Sock.Send(messageLength);
            Sock.Send(bytes);
        }

        private int GetMessageLength() {
            var bytes = new byte[sizeof(int)];
            Sock.Receive(bytes, sizeof(int), SocketFlags.None);
            var messageLength = BitConverter.ToInt32(bytes);
            return messageLength;
        }

        private byte[] GetMessage(int messageLength) {
            var message = new byte[messageLength];
            Sock.Receive(message, messageLength, SocketFlags.None);
            return message;
        }

        public byte[] Receive() {
            if (!Sock.Connected) {
                throw new SocketNotConnectedException();
            }

            var messageLength = GetMessageLength();
            if (messageLength == 0) {
                Sock.Close();
                throw new SocketClosedException();
            }

            return GetMessage(messageLength);
        }

        public void Close() {
            Sock.Close();
        }

        public override string ToString() {
            var remoteIpEndPoint = Sock.RemoteEndPoint as IPEndPoint;
            return $"IP: {remoteIpEndPoint.Address} | Port: {remoteIpEndPoint.Port.ToString()}";
        }
    }
}