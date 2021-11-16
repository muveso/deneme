using Google.Protobuf.WellKnownTypes;
using UnityEngine;

namespace Assets.Scripts.Game {
    public class Player : MonoBehaviour {
        public Rigidbody PlayerRigidbody;
        public float Speed;

        private void Update() {
            var moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
            // TODO: Send input
        }

        private Any ServerUpdate(Any message) {
            // parse moveDirection from message
            var moveDirection = new Vector3();
            PlayerRigidbody.velocity = moveDirection * Speed;

            // Serialize Back the state
            return new Any();
        }

        private void ClientUpdate(Any message) {
            // TODO: parse moveDirection from message
            var moveDirection = new Vector3();
            PlayerRigidbody.velocity = moveDirection * Speed;
        }
    }
}