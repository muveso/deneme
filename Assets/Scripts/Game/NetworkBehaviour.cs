using Assets.Scripts.General;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using UnityEngine;

namespace Assets.Scripts.Game {
    public abstract class NetworkBehaviour : MonoBehaviour {
        public bool IsLocal { get; set; } = false;
        public bool IsServer { get; set; } = false;

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

            if (IsServer) {
                ServerUpdate();
            }
        }

        /// <summary>
        ///     Create input message and send it to server
        /// </summary>
        /// <param name="message">The inner message to send to server</param>
        private void SendUpdate(IMessage message) {
            var inputMessage = new ObjectInputMessage {
                ObjectId = name,
                ClientId = GameManager.Instance.ClientId,
                Input = Any.Pack(message)
            };
            if (GameManager.Instance.NetworkManagers.UnreliableClientManager != null) {
                GameManager.Instance.NetworkManagers.UnreliableClientManager.Send(inputMessage);
            }
        }

        /// <summary>
        ///     Update the server with the input from the client
        /// </summary>
        /// <param name="message">The input message from client</param>
        public abstract void ServerUpdateFromClient(Any message);

        /// <summary>
        ///     The FixedUpdate logic on the server
        /// </summary>
        public abstract void ServerUpdate();

        /// <summary>
        ///     The FixedUpdate logic on the client
        /// </summary>
        /// <returns>Input message to send to server</returns>
        protected abstract IMessage ClientUpdate();

        /// <summary>
        ///     Gets state from server and deserialize it to the object
        /// </summary>
        /// <param name="message">The input message from client</param>
        public abstract void DeserializeState(Any message);

        /// <summary>
        ///     Serialize the object into message
        /// </summary>
        /// <returns>State message to send to client</returns>
        public abstract IMessage SerializeState();
    }
}