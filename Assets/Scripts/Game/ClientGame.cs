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
                new NetworkClientManager(udpClient);
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
                    foundGameObject = Player.CreatePlayer(
                        Vector3.zero,
                        objectStateMessage.ObjectId,
                        objectStateMessage.OwnerNickname,
                        objectStateMessage.OwnerNickname == GameManager.Instance.Nickname,
                        true);
                }

                foundGameObject.GetComponent<ISerializableNetworkObject>().DeserializeState(objectStateMessage.State);
            }
        }

        private void OnDestroy() {
            GameManager.Instance.Reset();
        }
    }
}