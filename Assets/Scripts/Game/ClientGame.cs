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
            foreach (var objectStateMessage in globalStateMessage.ObjectsState) {
                HandleState(objectStateMessage);
            }
        }

        private void HandleState(ObjectStateMessage objectStateMessage) {
            if (objectStateMessage.State.Is(PlayerStateMessage.Descriptor)) {
                var foundGameObject = GameObject.Find(objectStateMessage.ObjectId);
                if (foundGameObject == null) {
                    foundGameObject = CreatePlayer(objectStateMessage);
                }

                foundGameObject.GetComponent<NetworkBehaviour>().DeserializeState(objectStateMessage.State);
            }
        }

        private GameObject CreatePlayer(ObjectStateMessage objectStateMessage) {
            var playerPrefab = Resources.Load("Game/Prefabs/Player") as GameObject;
            var playerObject = Instantiate(playerPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            playerObject.name = objectStateMessage.ObjectId;
            playerObject.GetComponentInChildren<TextMesh>().text = objectStateMessage.OwnerNickname;
            if (objectStateMessage.OwnerNickname == GameManager.Instance.Nickname) {
                playerObject.AddComponent<Camera>();
                playerObject.GetComponent<NetworkBehaviour>().IsLocal = true;
            }

            return playerObject;
        }

        private void OnDestroy() {
            GameManager.Instance.Reset();
        }
    }
}