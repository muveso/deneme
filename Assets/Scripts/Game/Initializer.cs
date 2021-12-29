using System;
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
            CreateObstaclesObjects();
        }

        private void CreateObstaclesObjects() { }

        private void CreatePlayersObjects() {
            var startingPoint = GameObject.Find("StartingPoint").transform.position;
            foreach (var client in GameManager.Instance.NetworkManagers.TcpServerManager.Clients) {
                var playerObject =
                    Player.CreatePlayer(startingPoint,
                        Guid.NewGuid().ToString(),
                        client.Details.Nickname,
                        HostClient.IsHostClient(client));
                GameManager.Instance.ServerGameObjects[playerObject.name] =
                    new Tuple<GameObject, Client>(playerObject, client);
            }
        }
    }
}