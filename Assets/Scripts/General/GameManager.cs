using System;
using System.Net;
using Assets.Scripts.Network.Common;
using Assets.Scripts.Network.Server;

namespace Assets.Scripts.General {
    public class NetworkManagers : IDisposable {
        public IClientManager ReliableClientManager { get; set; }
        public IClientManager UnreliableClientManager { get; set; }
        public TcpServerManager TcpServerManager { get; set; }
        public UdpServerManager UdpServerManager { get; set; }

        public void Dispose() {
            if (ReliableClientManager != null) {
                ReliableClientManager.Dispose();
            }

            if (TcpServerManager != null) {
                TcpServerManager.Dispose();
            }

            if (UnreliableClientManager != null) {
                UnreliableClientManager.Dispose();
            }

            if (UdpServerManager != null) {
                UdpServerManager.Dispose();
            }
        }
    }

    public class GameManager {
        private static readonly object Padlock = new();
        private static GameManager _instance;

        private GameManager() {
            NetworkManagers = new NetworkManagers();
        }

        public IPAddress ServerIpAddress { get; set; }
        public NetworkManagers NetworkManagers { get; set; }
        public bool IsHost { get; set; }
        public string Nickname { get; set; }
        public string ClientId { get; set; }

        public static GameManager Instance {
            get {
                // There is no reason to lock if the instance is already initialized
                if (_instance == null) {
                    // Lock to make sure that only one thread will create instance
                    lock (Padlock) {
                        if (_instance == null) {
                            _instance = new GameManager();
                        }
                    }
                }

                return _instance;
            }
        }

        // Manual reset for singleton
        public void Reset() {
            IsHost = false;
            ServerIpAddress = null;
            NetworkManagers.Dispose();
            NetworkManagers = new NetworkManagers();
        }
    }
}