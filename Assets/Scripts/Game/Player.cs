using Assets.Scripts.General;
using Assets.Scripts.Utils.Messages;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using UnityEngine;

namespace Assets.Scripts.Game {
    public class Player : NetworkBehaviour {
        private float _distanceToGround;
        public float JumpForce;
        public string Nickname;
        public Rigidbody PlayerRigidbody;
        public float Speed;

        private void Awake() {
            _distanceToGround = GetComponent<Collider>().bounds.extents.y;
        }

        protected override IMessage ClientUpdate() {
            var moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
            if (Input.GetKey(KeyCode.Space)) {
                moveDirection.y = 1f;
            }

            if (moveDirection == Vector3.zero) {
                return null;
            }

            return new PlayerMovementMessage {
                KeyboardInput = MessagesHelpers.CreateVector3Message(moveDirection)
            };
        }

        public override void ServerUpdate(Any message) {
            var playerInput = message.Unpack<PlayerMovementMessage>().KeyboardInput;
            var moveDirection = MessagesHelpers.CreateVector3FromMessage(playerInput) * Speed;
            // Let y (height) be effected by gravity only
            PlayerRigidbody.velocity =
                new Vector3(moveDirection.x, PlayerRigidbody.velocity.y, moveDirection.z);

            if (moveDirection.y != 0 && IsGrounded()) {
                PlayerRigidbody.AddForce(Vector3.up * JumpForce, ForceMode.Impulse);
            }
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

        private bool IsGrounded() {
            return Physics.Raycast(transform.position, -Vector3.up, _distanceToGround + 0.1f);
        }

        /// <summary>
        ///     Checks if the player reached to the finish point and wins.
        ///     This function runs only on server.
        /// </summary>
        /// <param name="other"></param>
        private void OnTriggerEnter(Collider other) {
            if (other.name.Equals("FinishPoint")) {
                GameManager.Instance.IsGameEnded = true;
                GameManager.Instance.WinnerNickname = Nickname;
            }
        }

        public static GameObject CreatePlayer(Vector3 position, string name, string nickname, bool isLocal,
            bool disablePhysics = false) {
            Debug.Log("Player: creating new player");
            var playerPrefab = Resources.Load("Game/Prefabs/Player") as GameObject;
            var playerObject = Instantiate(playerPrefab, position, Quaternion.identity);
            playerObject.name = name;
            playerObject.GetComponentInChildren<TextMesh>().text = nickname;
            playerObject.GetComponentInChildren<Player>().Nickname = nickname;

            if (isLocal) {
                playerObject.GetComponentInChildren<NetworkBehaviour>().IsLocal = true;
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