using System.Net;
using Assets.Scripts.General;
using Assets.Scripts.Network.Client;
using UnityEngine;

namespace Assets.Scripts.Game {
    public class ClientGame : MonoBehaviour {
        private void Awake() {
            var endpoint =
                new IPEndPoint(NetworkManager.Instance.ServerIpAddress, GameConsts.DefaultUdpServerPort);
            NetworkManager.Instance.Communicators.UnreliableClientCommunicator = new UdpClientCommunicator(endpoint);
        }

        protected virtual void Update() {
            // Messages from server
            var message = NetworkManager.Instance.Communicators.UnreliableClientCommunicator.ReceiveAll();
            if (message != null) { }
        }

        private void OnDestroy() {
            NetworkManager.Instance.Reset();
        }
    }
}