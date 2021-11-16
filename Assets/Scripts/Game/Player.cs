using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using UnityEngine;

namespace Assets.Scripts.Game {
    public class Player : AbstractNetworkMonoBehaviour {
        public Rigidbody PlayerRigidbody;
        public float Speed;

        protected override IMessage ClientUpdate() {
            var moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
            return new ClientReadyMessage();
        }

        protected override void ServerUpdate(Any message) {
            // parse moveDirection from message
            var moveDirection = new Vector3();
            PlayerRigidbody.velocity = moveDirection * Speed;
        }

        protected override void UpdateStateFromServer(Any message) {
            // TODO: parse moveDirection from message
            var moveDirection = new Vector3();
            PlayerRigidbody.velocity = moveDirection * Speed;
        }
    }
}