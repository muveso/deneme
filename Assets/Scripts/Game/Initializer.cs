using Assets.Scripts.General;
using UnityEngine;

namespace Assets.Scripts.Game {
    public class Initializer : MonoBehaviour {
        private void Awake() {
            if (NetworkManager.Instance.IsHost) {
                gameObject.AddComponent<HostManager>();
            } else {
                gameObject.AddComponent<ClientManager>();
            }
        }
    }
}