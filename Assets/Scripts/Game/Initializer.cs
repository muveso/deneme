using UnityEngine;

namespace Evade.Game {
    public class Initializer : MonoBehaviour {
        private void Awake() {
            if (GameGlobals.IsHost) {
                gameObject.AddComponent<HostManager>();
            } else {
                gameObject.AddComponent<ClientManager>();
            }
        }
    }
}