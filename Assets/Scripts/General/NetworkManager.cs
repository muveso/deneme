using System;
using Assets.Scripts.Network.Client;
using Assets.Scripts.Network.Common;
using Assets.Scripts.Network.Server;

namespace Assets.Scripts.General {
    public class Communicators : IDisposable {
        public IClientCommunicator ClientCommunicator { get; set; }
        public TcpServerCommunicator TcpServerCommunicator { get; set; }
        public UdpClientCommunicator UdpClientCommunicator { get; set; }
        public UdpServerCommunicator UdpServerCommunicator { get; set; }

        public void Dispose() {
            if (ClientCommunicator != null) {
                ClientCommunicator.Dispose();
            }

            if (TcpServerCommunicator != null) {
                TcpServerCommunicator.Dispose();
            }

            if (UdpClientCommunicator != null) {
                UdpClientCommunicator.Dispose();
            }

            if (UdpServerCommunicator != null) {
                UdpServerCommunicator.Dispose();
            }
        }
    }

    public class NetworkManager {
        private static readonly object _padlock = new();
        private static NetworkManager _instance;

        private NetworkManager() {
            Communicators = new Communicators();
        }

        public Communicators Communicators { get; set; }
        public bool IsHost { get; set; }

        public static NetworkManager Instance {
            get {
                // There is no reason to lock if the instance is already initialized
                if (_instance == null) {
                    // Lock to make sure that only one thread will create instance
                    lock (_padlock) {
                        if (_instance == null) {
                            _instance = new NetworkManager();
                        }
                    }
                }

                return _instance;
            }
        }

        // Manual reset for singleton
        public void Reset() {
            IsHost = false;
            Communicators.Dispose();
            Communicators = new Communicators();
        }
    }
}