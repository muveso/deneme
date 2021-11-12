using UnityEngine;

namespace Evade.Game {
    public class Initializer : MonoBehaviour {
        private void Start() {
            if (GameManager.Instance.IsHost) {
                gameObject.AddComponent<HostManager>();
            } else {
                gameObject.AddComponent<ClientManager>();
            }
        }
    }
}