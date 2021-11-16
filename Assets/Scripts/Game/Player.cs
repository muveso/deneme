using Assets.Scripts.Utils;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using UnityEngine;

namespace Assets.Scripts.Game {
    public class Player : AbstractNetworkMonoBehaviour {
        public Rigidbody PlayerRigidbody;
        public float Speed;

        protected override IMessage ClientUpdate() {
            var moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
            if (moveDirection == Vector3.zero) {
                return null;
            }

            return new PlayerInputMessage {
                Input = MessagesHelpers.CreateVector3Message(moveDirection)
            };
        }

        public override void ServerUpdate(Any message) {
            var playerInput = message.Unpack<PlayerInputMessage>().Input;
            var moveDirection = MessagesHelpers.CreateVector3FromMessage(playerInput);
            PlayerRigidbody.velocity = moveDirection * Speed;
        }

        public override void DeserializeState(Any message) {
            var position = message.Unpack<PlayerStateMessage>().Position;
            PlayerRigidbody.position = MessagesHelpers.CreateVector3FromMessage(position);
        }

        public override IMessage SerializeState() {
            return new PlayerStateMessage {
                Position = MessagesHelpers.CreateVector3Message(PlayerRigidbody.position)
            };
        }
    }
}