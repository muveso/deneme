using System;
using Assets.Scripts.Game;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using UnityEngine;
using Random = UnityEngine.Random;

public class ObstacleOne : MonoBehaviour, ISerializableNetworkObject {
    private float _movementSpeed;

    public void DeserializeState(Any message) {
        throw new NotImplementedException();
    }

    public IMessage SerializeState() {
        throw new NotImplementedException();
    }

    private void Awake() {
        _movementSpeed = GetRandomMovementSpeed();
    }

    private void FixedUpdate() {
        transform.Translate(Vector3.right * _movementSpeed);
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
        return Random.Range(0.5f, 3f);
    }
}