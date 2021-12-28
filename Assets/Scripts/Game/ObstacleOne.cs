using System;
using Assets.Scripts.Game;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using UnityEngine;
using Random = UnityEngine.Random;

public class ObstacleOne : MonoBehaviour, ISerializableNetworkObject {
    private float _movementSpeed;
    private Rigidbody _rigidbody;

    public void DeserializeState(Any message) {
        throw new NotImplementedException();
    }

    public IMessage SerializeState() {
        throw new NotImplementedException();
    }

    private void Awake() {
        _rigidbody = GetComponent<Rigidbody>();
        _movementSpeed = GetRandomMovementSpeed();
    }

    private void FixedUpdate() {
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
}