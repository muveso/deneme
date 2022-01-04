using System;
using Assets.Scripts.General;
using Assets.Scripts.Network.Common;
using Tayx.Graphy;
using UnityEngine;
using Random = System.Random;

namespace Assets.Scripts.Game {
    public class Initializer : MonoBehaviour {
        private void Awake() {
            GraphyManager.Instance.Enable();
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
            var startingPoint = GetStartingPoint().transform.position;

            for (var i = 1; i <= GameConsts.ObstacleCount; ++i) {
                var obstacleOneObject = ObstacleOne.CreateObstacleOne(startingPoint + Vector3.forward * i * 30,
                    Guid.NewGuid().ToString(), true);
                GameManager.Instance.ServerGameObjects[obstacleOneObject.name] =
                    new Tuple<GameObject, Client>(obstacleOneObject, serverClient);
            }
        }

        private void CreatePlayersObjects() {
            foreach (var client in GameManager.Instance.NetworkManagers.TcpServerManager.Clients) {
                var playerObject =
                    Player.CreatePlayer(GetRandomPositionInStartingPoint(),
                        Guid.NewGuid().ToString(),
                        client.Details.Nickname,
                        HostClient.IsHostClient(client));
                GameManager.Instance.ServerGameObjects[playerObject.name] =
                    new Tuple<GameObject, Client>(playerObject, client);
            }
        }

        private Vector3 GetRandomPositionInStartingPoint() {
            var startingPoint = GetStartingPoint();
            var verticesList = startingPoint.GetComponent<MeshFilter>().sharedMesh.vertices;

            var rightUp = startingPoint.transform.TransformPoint(verticesList[0]);
            var leftUp = startingPoint.transform.TransformPoint(verticesList[10]);
            var rightBottom = startingPoint.transform.TransformPoint(verticesList[110]);

            var random = new Random();
            return new Vector3(
                random.Next((int) leftUp.x, (int) rightUp.x),
                leftUp.y,
                random.Next((int) rightBottom.z, (int) rightUp.z)
            );
        }

        private GameObject GetStartingPoint() {
            return GameObject.Find("StartingPoint");
        }
    }
}