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
            if (messages != null) {
                foreach (var message in messages) {
                    HandleMessage(message);
                    SendGlobalStateToAllClients();
                }
            }
        }

        private void SendGlobalStateToAllClients() {
            var globalState = GetGlobalState();
            GameManager.Instance.NetworkManagers.UdpServerManager.Communicator.Send(globalState);
        }

        private IMessage GetGlobalState() {
            var globalStateMessage = new GlobalStateMessage();
            foreach (var client in GameManager.Instance.NetworkManagers.TcpServerManager.Clients) {
                var messageToPack = GameObject.Find(client.Id).GetComponent<Player>().SerializeState();
                var stateMessage = new PlayerStateMessage {
                    Nickname = client.Details.Nickname,
                    State = Any.Pack(messageToPack)
                };
                globalStateMessage.ObjectsState.Add(Any.Pack(stateMessage));
            }

            return globalStateMessage;
        }

        private void HandleMessage(MessageToReceive messageToReceive) {
            var anyMessage = messageToReceive.AnyMessage;
            if (!anyMessage.Is(PlayerInputMessage.Descriptor)) {
                throw new Exception("Got unsupported message from client");
            }

            var playerInputMessage = anyMessage.Unpack<PlayerInputMessage>();
            GameObject.Find(playerInputMessage.ClientId).GetComponent<Player>()
                .ServerUpdate(playerInputMessage.Input);
        }


        private void OnDestroy() {
            GameManager.Instance.Reset();
        }
    }
}