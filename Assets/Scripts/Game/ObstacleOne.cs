using System;
using Assets.Scripts.Game;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using UnityEngine;
using Random = UnityEngine.Random;

public class ObstacleOne : NetworkBehaviour {
    private float _movementSpeed;
    private Rigidbody _rigidbody;


    private void Awake() {
        _rigidbody = GetComponent<Rigidbody>();
        _movementSpeed = GetRandomMovementSpeed();
    }

    public override void DeserializeState(Any message) {
        throw new NotImplementedException();
    }

    public override IMessage SerializeState() {
        throw new NotImplementedException();
    }

    protected override IMessage ClientUpdate() {
        throw new NotImplementedException();
    }

    public override void ServerUpdate(Any message) {
        _rigidbody.velocity = Vector3.right * _movementSpeed;
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Border")) {
            var newMovementSpeed = GetRandomMovementSpeed();
            if (_movementSpeed > 0) {
                _movementSpeed = -newMovementSpeed;
            } else {
                _movementSpeed = newMovementSpeed;
            }
        }
    }

    private float GetRandomMovementSpeed() {
        return Random.Range(20f, 40f);
    }

    public static GameObject CreateObstacleOne(Vector3 position, string name, bool isServer,
        bool disablePhysics = false) {
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