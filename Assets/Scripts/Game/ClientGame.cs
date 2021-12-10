using System.Net;
using Assets.Scripts.General;
using Assets.Scripts.Network.Client;
using Assets.Scripts.Utils.Network.UDP;
using Google.Protobuf.WellKnownTypes;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Game {
    public class ClientGame : MonoBehaviour {
        private Player _player;

        private void Awake() {
            _player = GameObject.FindWithTag("Player").GetComponent<Player>();
            var endpoint =
                new IPEndPoint(NetworkManager.Instance.ServerIpAddress, GameConsts.DefaultUdpServerPort);
            var udpClient = new UdpClientMessageBasedClient(new UdpClient(endpoint));
            NetworkManager.Instance.Communicators.UnreliableClientCommunicator =
                new NetworkClientManager(udpClient);
            NetworkManager.Instance.Communicators.UnreliableClientCommunicator.Send(new ClientReadyMessage());
        }

        protected virtual void FixedUpdate() {
            // Messages from server
            var message = NetworkManager.Instance.Communicators.UnreliableClientCommunicator.Receive();
            if (message == null) {
                return;
            }


            if (message.AnyMessage.Is(ServerDisconnectMessage.Descriptor)) {
                SceneManager.LoadScene("MainMenu");
            } else {
                HandleGlobalState(message.AnyMessage.Unpack<GlobalStateMessage>());
            }
        }

        private void HandleGlobalState(GlobalStateMessage globalStateMessage) {
            foreach (var state in globalStateMessage.ObjectsState) {
                HandleState(state);
            }
        }

        private void HandleState(Any state) {
            _player.DeserializeState(state);
        }

        private void OnDestroy() {
            NetworkManager.Instance.Reset();
        }
    }
}