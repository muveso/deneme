using Assets.Scripts.General;
using Tayx.Graphy;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.Game {
    public class GameEnded : MonoBehaviour {
        private void Awake() {
            GraphyManager.Instance.Disable();
            var winnerNickname = GameObject.Find("WinnerNickname").GetComponent<TextMeshProUGUI>();
            winnerNickname.text = GameManager.Instance.WinnerNickname;
        }

        private void OnDestroy() {
            GameManager.Instance.Reset();
        }
    }
}