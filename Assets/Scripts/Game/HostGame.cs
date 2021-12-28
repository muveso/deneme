using System;
using Assets.Scripts.General;
using Assets.Scripts.Network.Host;
using Assets.Scripts.Network.Server;
using Assets.Scripts.Utils.Messages;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using UnityEngine;

namespace Assets.Scripts.Game {
    public class HostGame : MonoBehaviour {
        private void Awake() {
            GameManager.Instance.NetworkManagers.UdpServerManager =
                new UdpServerManager(GameConsts.DefaultUdpServerPort);
            GameManager.Instance.NetworkManagers.UnreliableClientManager =
                new HostClientManager(GameManager.Instance.NetworkManagers.UdpServerManager,
                    GameManager.Instance.Nickname);
        }

        private void FixedUpdate() {
            // All the game logic is here
            // Host does not need to get messages from server like the client because he is the server
            var messages = GameManager.Instance.NetworkManagers.UdpServerManager.Communicator.ReceiveAll();
            if (messages.Count > 0) {
                foreach (var message in messages) {
                    HandleMessage(message);
                }
            }

            SendGlobalStateToAllClients();
        }

        private void SendGlobalStateToAllClients() {
            var globalState = GetGlobalState();
            GameManager.Instance.NetworkManagers.UdpServerManager.Communicator.Send(globalState);
        }

        private IMessage GetGlobalState() {
            var globalStateMessage = new GlobalStateMessage();
            foreach (var serverGameObject in GameManager.Instance.ServerGameObjects) {
                var objectId = serverGameObject.Key;
                var realGameObject = serverGameObject.Value.Item1;
                var ownerNickname = serverGameObject.Value.Item2;

                var messageToPack = realGameObject.GetComponent<NetworkBehaviour>().SerializeState();
                var stateMessage = new ObjectStateMessage {
                    ObjectId = objectId,
                    OwnerNickname = ownerNickname,
                    State = Any.Pack(messageToPack)
                };
                globalStateMessage.ObjectsState.Add(stateMessage);
            }

            return globalStateMessage;
        }

        private void HandleMessage(MessageToReceive messageToReceive) {
            var anyMessage = messageToReceive.AnyMessage;
            if (!anyMessage.Is(ObjectInputMessage.Descriptor)) {
                throw new Exception("Got unsupported message from client");
            }

            var objectInputMessage = anyMessage.Unpack<ObjectInputMessage>();

            GameManager.Instance.ServerGameObjects[objectInputMessage.ObjectId].Item1
                .GetComponent<NetworkBehaviour>()
                .ServerUpdate(objectInputMessage.Input);
        }


        private void OnDestroy() {
            GameManager.Instance.Reset();
        }
    }
}