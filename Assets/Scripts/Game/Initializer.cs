using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Evade.Game {
    public class Initializer : MonoBehaviour {
        void Start() {
            GameObject manager = new GameObject();
            if (GameManager.Instance.IsHost) {
                manager.AddComponent<HostManager>();
            } else {
                manager.AddComponent<ClientManager>();
            }
            Instantiate(manager, Vector3.zero, Quaternion.identity);
        }
    }
}
