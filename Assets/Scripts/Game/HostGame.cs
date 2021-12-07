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
        private DateTime _lastSendDateTime = DateTime.MinValue;

        private void Awake() {
            NetworkManager.Instance.Communicators.UdpServerCommunicator =
                new UdpServerCommunicator(GameConsts.DefaultUdpServerPort);
            NetworkManager.Instance.Communicators.UnreliableClientCommunicator =
                new HostClientCommunicator(NetworkManager.Instance.Communicators.UdpServerCommunicator,
                    ClientGlobals.Nickname);
        }

        private void Update() {
            // All the game logic is here
            // Host does not need to get messages from server like the client because he is the server
            var messages = NetworkManager.Instance.Communicators.UdpServerCommunicator.ReceiveAll();
            if (messages != null) {
                foreach (var message in messages) {
                    HandleMessage(message);
                    SendGlobalStateToAllClients();
                }
            }
        }

        private void SendGlobalStateToAllClients() {
            var currentDateTime = DateTime.Now;
            if (true) {
                _lastSendDateTime = currentDateTime;
                var globalState = GetGlobalState();
                NetworkManager.Instance.Communicators.UdpServerCommunicator.SendAll(globalState);
            }
        }

        private IMessage GetGlobalState() {
            var globalStateMessage = new GlobalStateMessage();
            var messageToPack = GameObject.FindWithTag("Player").GetComponent<Player>().SerializeState();
            globalStateMessage.ObjectsState.Add(Any.Pack(messageToPack));
            return globalStateMessage;
        }

        private void HandleMessage(MessageToReceive messageToReceive) {
            var anyMessage = messageToReceive.AnyMessage;
            if (anyMessage.Is(PlayerInputMessage.Descriptor)) {
                GameObject.FindWithTag("Player").GetComponent<Player>().ServerUpdate(anyMessage);
            }
        }

        private void OnDestroy() {
            NetworkManager.Instance.Reset();
        }
    }
}