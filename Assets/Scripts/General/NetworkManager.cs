using System;
using System.Net;
using Assets.Scripts.Network.Common;
using Assets.Scripts.Network.Server;

namespace Assets.Scripts.General {
    public class Communicators : IDisposable {
        public IClientCommunicator TcpClientCommunicator { get; set; }
        public TcpServerCommunicator TcpServerCommunicator { get; set; }
        public IClientCommunicator UdpClientCommunicator { get; set; }
        public UdpServerCommunicator UdpServerCommunicator { get; set; }

        public void Dispose() {
            if (TcpClientCommunicator != null) {
                TcpClientCommunicator.Dispose();
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

        public IPAddress ServerIpAddress { get; set; }
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
            ServerIpAddress = null;
            Communicators.Dispose();
            Communicators = new Communicators();
        }
    }
}