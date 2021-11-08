using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Evade.Game {
    public class Initializer : MonoBehaviour {
        void Start() {
            if (GameManager.Instance.IsHost) {
                gameObject.AddComponent<HostManager>();
            } else {
                gameObject.AddComponent<ClientManager>();
            }
        }
    }
}
