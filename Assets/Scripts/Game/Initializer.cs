using System;
using Assets.Scripts.General;
using Assets.Scripts.Network.Common;
using UnityEngine;

namespace Assets.Scripts.Game {
    public class Initializer : MonoBehaviour {
        private void Awake() {
            Time.fixedDeltaTime = GameConsts.GameRate;
            if (GameManager.Instance.IsHost) {
                gameObject.AddComponent<HostGame>();
                CreateGameObjects();
            } else {
                gameObject.AddComponent<ClientGame>();
            }
        }

        private void CreateGameObjects() {
            CreatePlayersObjects();
            CreateObstaclesObjects();
        }

        private void CreateObstaclesObjects() {
            var serverClient = new ServerClient();
            var startingPoint = GameObject.Find("StartingPoint").transform.position;

            for (var i = 1; i <= GameConsts.ObstacleCount; ++i) {
                var obstacleOneObject = ObstacleOne.CreateObstacleOne(startingPoint + Vector3.forward * i * 30,
                    Guid.NewGuid().ToString(), true);
                GameManager.Instance.ServerGameObjects[obstacleOneObject.name] =
                    new Tuple<GameObject, Client>(obstacleOneObject, serverClient);
            }
        }

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