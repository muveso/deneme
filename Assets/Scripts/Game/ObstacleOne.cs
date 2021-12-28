using System;
using Assets.Scripts.Game;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using UnityEngine;

public class ObstacleOne : MonoBehaviour, ISerializableNetworkObject {
    public void DeserializeState(Any message) {
        throw new NotImplementedException();
    }

    public IMessage SerializeState() {
        throw new NotImplementedException();
    }

    private void FixedUpdate() { }
}