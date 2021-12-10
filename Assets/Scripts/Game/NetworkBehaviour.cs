using Assets.Scripts.General;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using UnityEngine;

namespace Assets.Scripts.Game {
    public abstract class NetworkBehaviour : MonoBehaviour {
        protected bool IsLocal {
            get {
                if (GameManager.Instance.IsHost) {
                    return name == GameManager.Instance.ClientId;
                }

                return name == GameManager.Instance.Nickname;
            }
        }

        /// <summary>
        ///     FixedUpdate used in order to sync the input speed from clients.
        ///     If one client will run on higher fps we don't want it to send more input to the server.
        /// </summary>
        protected virtual void FixedUpdate() {
            if (IsLocal) {
                var updateMessage = ClientUpdate();
                if (updateMessage != null) {
                    SendUpdate(updateMessage);
                }
            }
        }

        private void SendUpdate(IMessage message) {
            var inputMessage = new PlayerInputMessage {
                ClientId = GameManager.Instance.ClientId,
                Input = Any.Pack(message)
            };
            GameManager.Instance.NetworkManagers.UnreliableClientManager.Send(inputMessage);
        }

        public abstract void ServerUpdate(Any message);
        public abstract void DeserializeState(Any message);
        public abstract IMessage SerializeState();
        protected abstract IMessage ClientUpdate();
    }
}