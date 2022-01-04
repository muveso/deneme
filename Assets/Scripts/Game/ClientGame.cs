using System;
using System.Linq;
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
            HandleUnreliableMessages();
            HandleReliableMessages();
        }

        private void HandleReliableMessages() {
            var messages = GameManager.Instance.NetworkManagers.ReliableClientManager.ReceiveAll();
            if (!messages.Any()) {
                return;
            }

            foreach (var message in messages) {
                if (message.AnyMessage.Is(GameEndedMessage.Descriptor)) {
                    GameManager.Instance.WinnerNickname = message.AnyMessage.Unpack<GameEndedMessage>().WinnerNickname;
                    SceneManager.LoadScene("GameEnd");
                    Destroy(this);
                } else {
                    Debug.Log("ClientGame: HandleReliableMessages got unknown message");
                }
            }
        }

        private void HandleUnreliableMessages() {
            var message = GameManager.Instance.NetworkManagers.UnreliableClientManager.Receive();
            if (message == null) {
                return;
            }

            if (message.AnyMessage.Is(ServerDisconnectMessage.Descriptor)) {
                SceneManager.LoadScene("MainMenu");
                Destroy(this);
            } else {
                HandleGameState(message.AnyMessage.Unpack<GlobalStateMessage>());
            }
        }

        private void HandleGameState(GlobalStateMessage globalStateMessage) {
            foreach (var objectStateMessage in globalStateMessage.ObjectsState) {
                try {
                    HandleObjectState(objectStateMessage);
                } catch (Exception e) {
                    Debug.Log(e);
                }
            }
        }

        private void HandleObjectState(ObjectStateMessage objectStateMessage) {
            var foundGameObject = GameObject.Find(objectStateMessage.ObjectId);
            if (objectStateMessage.State.Is(PlayerStateMessage.Descriptor)) {
                if (foundGameObject == null) {
                    foundGameObject = Player.CreatePlayer(
                        Vector3.zero,
                        objectStateMessage.ObjectId,
                        objectStateMessage.OwnerNickname,
                        objectStateMessage.OwnerNickname == GameManager.Instance.Nickname,
                        true);
                }
            } else if (objectStateMessage.State.Is(ObstacleStateMessage.Descriptor)) {
                if (foundGameObject == null) {
                    foundGameObject = ObstacleOne.CreateObstacleOne(
                        Vector3.zero,
                        objectStateMessage.ObjectId,
                        false,
                        true);
                }
            } else {
                throw new Exception("Got unsupported state message");
            }

            foundGameObject.GetComponentInChildren<NetworkBehaviour>().DeserializeState(objectStateMessage.State);
        }
    }
}