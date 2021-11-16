using System.Net;
using Assets.Scripts.General;
using Assets.Scripts.Network.Client;
using UnityEngine;

namespace Assets.Scripts.Game {
    public class ClientGame : MonoBehaviour {
        private void Awake() {
            var endpoint =
                new IPEndPoint(NetworkManager.Instance.ServerIpAddress, GameConsts.DefaultUdpServerPort);
            NetworkManager.Instance.Communicators.UdpClientCommunicator = new UdpClientCommunicator(endpoint);
        }

        protected virtual void Update() {
            // Messages from server
            var message = NetworkManager.Instance.Communicators.UdpClientCommunicator.GetMessage();
            if (message != null) { }
        }
    }
}