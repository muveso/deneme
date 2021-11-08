using System;

namespace Evade {
    public class Communicators : IDisposable {
        public TcpClientCommunicator TcpClientCommunicator { get; set; }
        public TcpServerCommunicator TcpServerCommunicator { get; set; }
        public UdpCommunicator UdpClientCommunicator { get; set; }
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

    public class GameManager {
        private static readonly object _padlock = new object();
        private static GameManager instance = null;
        public Communicators Communicators { get; set; }
        public bool IsHost { get; set; } = false;

        GameManager() {
            Communicators = new Communicators();
        }

        // Extra safety to make sure all singleton resources are disposed
        ~GameManager() {
            Reset();
        }

        // Manual reset for singleton
        public void Reset() {
            IsHost = false;
            Communicators.Dispose();
            Communicators = new Communicators();
        }

        public static GameManager Instance {
            get {
                // There is no reason to lock if the instance is already initialized
                if (instance == null) {
                    // Lock to make sure that only one thread will create instance
                    lock (_padlock) {
                        if (instance == null) {
                            instance = new GameManager();
                        }
                    }
                }
                return instance;
            }
        }
    }
}