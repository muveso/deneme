using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Utils.UI {
    public static class General {
        public static void DestroyAllChildren(Transform transform) {
            foreach (Transform child in transform) {
                GameObject.Destroy(child.gameObject);
            }
        }
    }
}