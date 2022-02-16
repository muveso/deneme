using System;
using Assets.Scripts.Game;
using Assets.Scripts.Utils.Messages;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using UnityEngine;
using Random = UnityEngine.Random;

public class ObstacleOne : NetworkBehaviour {
    private const float MinimumSpeed = 10f;
    private const float MaximumSpeed = 30f;
    private float _movementSpeed;
    private Rigidbody _rigidbody;

    private void Awake() {
        _rigidbody = GetComponent<Rigidbody>();
        _movementSpeed = GetRandomMovementSpeed();
    }

    private void OnTriggerEnter(Collider other) {
        if (!IsServer) {
            return;
        }

        if (other.CompareTag("Border")) {
            var newMovementSpeed = GetRandomMovementSpeed();
            if (_movementSpeed > 0) {
                _movementSpeed = -newMovementSpeed;
            } else {
                _movementSpeed = newMovementSpeed;
            }
        }
    }

    public override void DeserializeState(Any message) {
        var obstacleStateMessage = message.Unpack<ObstacleStateMessage>();
        transform.position = MessagesHelpers.CreateVector3FromMessage(obstacleStateMessage.Position);
        _rigidbody.velocity = MessagesHelpers.CreateVector3FromMessage(obstacleStateMessage.Velocity);
    }

    public override IMessage SerializeState() {
        var obstacleStateMessage = new ObstacleStateMessage {
            Position = MessagesHelpers.CreateVector3Message(transform.position),
            Velocity = MessagesHelpers.CreateVector3Message(_rigidbody.velocity)
        };
        return obstacleStateMessage;
    }

    protected override IMessage ClientUpdate() {
        throw new NotImplementedException();
    }

    public override void ServerUpdate() {
        _rigidbody.velocity = Vector3.right * _movementSpeed;
    }

    public override void ServerUpdateFromClient(Any message) { }

    private float GetRandomMovementSpeed() {
        return Random.Range(MinimumSpeed, MaximumSpeed);
    }

    public static GameObject CreateObstacleOne(Vector3 position, string name, bool isServer,
        bool disablePhysics = false) {
        Debug.Log("ObstacleOne: creating new obstacle from type one");
        var obstacleOnePrefab = Resources.Load("Game/Prefabs/ObstacleOne") as GameObject;
        var obstacleOneObject = Instantiate(obstacleOnePrefab, position, Quaternion.identity);
        obstacleOneObject.name = name;

        if (isServer) {
            obstacleOneObject.GetComponentInChildren<NetworkBehaviour>().IsServer = true;
        }

        if (disablePhysics) {
            obstacleOneObject.GetComponentInChildren<Rigidbody>().isKinematic = true;
        }

        return obstacleOneObject;
    }
}