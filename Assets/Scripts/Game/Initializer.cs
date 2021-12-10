using Assets.Scripts.General;
using Assets.Scripts.Network.Common;
using UnityEngine;

namespace Assets.Scripts.Game {
    public class Initializer : MonoBehaviour {
        private void Awake() {
            if (GameManager.Instance.IsHost) {
                CreateGameObjects();
                gameObject.AddComponent<HostGame>();
            } else {
                gameObject.AddComponent<ClientGame>();
            }
        }

        private void CreateGameObjects() {
            CreatePlayersObjects();
        }

        private void CreatePlayersObjects() {
            var playerPrefab = Resources.Load("Game/Prefabs/Player") as GameObject;
            foreach (var client in GameManager.Instance.NetworkManagers.TcpServerManager.Clients) {
                var player = Instantiate(playerPrefab, new Vector3(0, 1, -1), Quaternion.identity);
                player.name = client.Id;
                if (HostClient.IsHostClient(client)) {
                    player.AddComponent<Camera>();
                }
            }
        }
    }
}