using UnityEngine;

namespace Evade {
    public class GameManager : MonoBehaviour {
        public TcpClientCommunicator TcpClientCommunicator { get; set; }
        public TcpServerCommunicator TcpServerCommunicator { get; set; }
        public UdpCommunicator UdpClientCommunicator { get; set; }
        public UdpServerCommunicator UdpServerCommunicator { get; set; }
        public bool IsHost { get; set; } = false;
        // public static GameManager Instance { get; private set; }

        private void Awake() {
            DontDestroyOnLoad(gameObject);
        }
        
        private void OnDestroy() {
            if (TcpClientCommunicator != null) {
                TcpClientCommunicator.Stop();
            }
            if (TcpServerCommunicator != null) {
                TcpServerCommunicator.Stop();
            }
            if (UdpClientCommunicator != null) {
                UdpClientCommunicator.Stop();
            }
            if (UdpServerCommunicator != null) {
                UdpServerCommunicator.Stop();
            }
        }

        /*
        private void Awake() {
            if (Instance == null) {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            } else if (Instance != this) {
                // If we return to the Scene where this script is attached to (MainMenu), Awake will be called again
                // so we need to destroy the new instance to avoid multiple GameManager instances.
                Destroy(gameObject);
            }
        }

        private void OnDestroy() {
            if (Instance.TcpClientCommunicator != null) {
                Instance.TcpClientCommunicator.Stop();
            }
            if (Instance.TcpServerCommunicator != null) {
                Instance.TcpServerCommunicator.Stop();
            }
            if (Instance.UdpClientCommunicator != null) {
                Instance.UdpClientCommunicator.Stop();
            }
            if (Instance.UdpServerCommunicator != null) {
                Instance.UdpServerCommunicator.Stop();
            }
        }*/
    }
}