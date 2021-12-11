using System;
using Assets.Scripts.Network.Common;
using Assets.Scripts.Network.Server;

namespace Assets.Scripts.General {
    public class NetworkManagers : IDisposable {
        public IClientManager ReliableClientManager { get; set; }
        public IClientManager UnreliableClientManager { get; set; }
        public TcpServerManager TcpServerManager { get; set; }
        public UdpServerManager UdpServerManager { get; set; }

        public void Dispose() {
            ReliableClientManager?.Dispose();
            TcpServerManager?.Dispose();
            UnreliableClientManager?.Dispose();
            UdpServerManager?.Dispose();
        }
    }
}