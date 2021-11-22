using Assets.Scripts.General;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using UnityEngine;

namespace Assets.Scripts.Game {
    public abstract class AbstractNetworkMonoBehaviour : MonoBehaviour {
        protected bool IsLocal { get; set; } = true;

        private void Update() {
            if (IsLocal) {
                var updateMessage = ClientUpdate();
                if (updateMessage != null) {
                    NetworkManager.Instance.Communicators.UnreliableClientCommunicator.Send(updateMessage);
                }
            }
        }

        public abstract void ServerUpdate(Any message);
        public abstract void DeserializeState(Any message);
        public abstract IMessage SerializeState();
        protected abstract IMessage ClientUpdate();
    }
}