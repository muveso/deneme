using System;
using Assets.Scripts.General;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using UnityEngine;

namespace Assets.Scripts.Game {
    public abstract class NetworkBehaviour : MonoBehaviour {
        private DateTime _lastSendDateTime = DateTime.MinValue;
        protected bool IsLocal { get; set; } = true;

        private void Update() {
            if (IsLocal) {
                var updateMessage = ClientUpdate();
                if (updateMessage != null) {
                    SendUpdate(updateMessage);
                }
            }
        }

        private void SendUpdate(IMessage message) {
            var currentDateTime = DateTime.Now;
            if (true) {
                _lastSendDateTime = currentDateTime;
                NetworkManager.Instance.Communicators.UnreliableClientCommunicator.Send(message);
            }
        }

        public abstract void ServerUpdate(Any message);
        public abstract void DeserializeState(Any message);
        public abstract IMessage SerializeState();
        protected abstract IMessage ClientUpdate();
    }
}