using Assets.Scripts.General;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using UnityEngine;

namespace Assets.Scripts.Game {
    public abstract class AbstractNetworkMonoBehaviour : MonoBehaviour {
        protected bool IsLocal { get; set; }

        private void Update() {
            if (IsLocal) {
                NetworkManager.Instance.Communicators.TcpClientCommunicator.Send(ClientUpdate());
            }
        }

        protected abstract void ServerUpdate(Any message);
        protected abstract void UpdateStateFromServer(Any message);
        protected abstract IMessage ClientUpdate();
    }
}