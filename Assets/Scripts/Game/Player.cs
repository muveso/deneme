using Assets.Scripts.Utils.Messages;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using UnityEngine;

namespace Assets.Scripts.Game {
    public class Player : ClientNetworkBehaviour {
        public Rigidbody PlayerRigidbody;
        public float Speed;

        protected override IMessage ClientUpdate() {
            var moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
            if (moveDirection == Vector3.zero) {
                return null;
            }

            return new PlayerMovementMessage {
                KeyboardInput = MessagesHelpers.CreateVector3Message(moveDirection)
            };
        }

        public override void ServerUpdate(Any message) {
            var playerInput = message.Unpack<PlayerMovementMessage>().KeyboardInput;
            var moveDirection = MessagesHelpers.CreateVector3FromMessage(playerInput);
            PlayerRigidbody.velocity = moveDirection * Speed;
        }

        public override void DeserializeState(Any message) {
            var playerStateMessage = message.Unpack<PlayerStateMessage>();
            transform.position = MessagesHelpers.CreateVector3FromMessage(playerStateMessage.Position);
            PlayerRigidbody.velocity = MessagesHelpers.CreateVector3FromMessage(playerStateMessage.Velocity);
        }

        public override IMessage SerializeState() {
            var playerStateMessage = new PlayerStateMessage {
                Position = MessagesHelpers.CreateVector3Message(transform.position),
                Velocity = MessagesHelpers.CreateVector3Message(PlayerRigidbody.velocity)
            };
            return playerStateMessage;
        }

        public static GameObject CreatePlayer(Vector3 position, string name, string nickname, bool isLocal,
            bool disablePhysics = false) {
            var playerPrefab = Resources.Load("Game/Prefabs/Player") as GameObject;
            var playerObject = Instantiate(playerPrefab, position, Quaternion.identity);
            playerObject.name = name;
            playerObject.GetComponentInChildren<TextMesh>().text = nickname;

            if (isLocal) {
                playerObject.GetComponent<ClientNetworkBehaviour>().IsLocal = true;
            } else {
                playerObject.GetComponentInChildren<Camera>().gameObject.SetActive(false);
            }

            if (disablePhysics) {
                playerObject.GetComponent<Rigidbody>().isKinematic = true;
            }

            return playerObject;
        }
    }
}