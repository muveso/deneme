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
        }

        private void CreatePlayersObjects() {
            var startingPoint = GameObject.Find("StartingPoint").transform;
            var playerPrefab = Resources.Load("Game/Prefabs/Player") as GameObject;
            foreach (var client in GameManager.Instance.NetworkManagers.TcpServerManager.Clients) {
                var playerObject = Instantiate(playerPrefab, startingPoint.position, Quaternion.identity);
                playerObject.name = Guid.NewGuid().ToString();
                playerObject.GetComponentInChildren<TextMesh>().text = client.Details.Nickname;
                if (HostClient.IsHostClient(client)) {
                    playerObject.AddComponent<Camera>();
                    playerObject.GetComponent<ClientNetworkBehaviour>().IsLocal = true;
                }

                GameManager.Instance.ServerGameObjects[playerObject.name] =
                    new Tuple<GameObject, string>(playerObject, client.Details.Nickname);
            }
        }
    }
}