using System.Net;
using Assets.Scripts.General;
using Assets.Scripts.Network.Client;
using Assets.Scripts.Utils.Network.UDP;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Game {
    public class ClientGame : MonoBehaviour {
        private void Awake() {
            var endpoint =
                new IPEndPoint(GameManager.Instance.ServerIpAddress, GameConsts.DefaultUdpServerPort);
            var udpClient = new UdpClientMessageBasedClient(new UdpClient(endpoint));
            GameManager.Instance.NetworkManagers.UnreliableClientManager =
                new NetworkClientClientManager(udpClient);
            GameManager.Instance.NetworkManagers.UnreliableClientManager.Send(new ClientReadyMessage());
        }

        protected virtual void FixedUpdate() {
            // Messages from server
            var message = GameManager.Instance.NetworkManagers.UnreliableClientManager.Receive();
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
                HandleState(state.Unpack<PlayerStateMessage>());
            }
        }

        private void HandleState(PlayerStateMessage state) {
            var player = GameObject.Find(state.Nickname);
            if (player == null) {
                player = CreatePlayer(state);
            }

            player.GetComponent<Player>().DeserializeState(state.State);
        }

        private GameObject CreatePlayer(PlayerStateMessage state) {
            var playerPrefab = Resources.Load("Game/Prefabs/Player") as GameObject;
            var player = Instantiate(playerPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            player.name = state.Nickname;
            if (player.name == GameManager.Instance.Nickname) {
                player.AddComponent<Camera>();
            }

            return player;
        }

        private void OnDestroy() {
            GameManager.Instance.Reset();
        }
    }
}