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
                new HostClientClientManager(GameManager.Instance.NetworkManagers.UdpServerManager,
                    ClientGlobals.Nickname);
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
            GameManager.Instance.Reset();
        }
    }
}