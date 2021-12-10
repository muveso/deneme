using Assets.Scripts.General;
using UnityEngine;

namespace Assets.Scripts.Game {
    public class Initializer : MonoBehaviour {
        private void Awake() {
            if (GameManager.Instance.IsHost) {
                gameObject.AddComponent<HostGame>();
            } else {
                gameObject.AddComponent<ClientGame>();
            }
        }
    }
}