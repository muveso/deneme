using System;
using System.Net;
using System.Net.Sockets;

namespace Network.Utils {
    public class TcpClient {
        public Socket Sock { get; private set; }

        public TcpClient(IPEndPoint endpoint) {
            Sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Sock.Connect(endpoint);
        }

        public TcpClient(Socket socket) {
            Sock = socket;
        }

        public void Send(byte[] bytes) {
            if (!Sock.Connected) {
                throw new SocketNotConnectedException();
            }
            byte[] messageLength = BitConverter.GetBytes(bytes.Length);
            Sock.Send(messageLength);
            Sock.Send(bytes);
        }

        private int GetMessageLength() {
            byte[] bytes = new byte[sizeof(int)];
            Sock.Receive(bytes, sizeof(int), SocketFlags.None);
            int messageLength = BitConverter.ToInt32(bytes);
            return messageLength;
        }

        private byte[] GetMessage(int messageLength) {
            byte[] message = new byte[messageLength];
            Sock.Receive(message, messageLength, SocketFlags.None);
            return message;
        }

        public byte[] Recieve() {
            if (!Sock.Connected) {
                throw new SocketNotConnectedException();
            }
            int messageLength = GetMessageLength();
            if (messageLength == 0) {
                Sock.Close();
                throw new SocketClosedException();
            }
            return GetMessage(messageLength);
        }

        public void Close() {
            Sock.Close();
        }
    }
}