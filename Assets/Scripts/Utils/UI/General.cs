using UnityEngine;

namespace Assets.Scripts.Utils.UI {
    public static class General {
        public static void DestroyAllChildren(Transform transform) {
            foreach (Transform child in transform) {
                Object.Destroy(child.gameObject);
            }
        }
    }
}