using Assets.Scripts.General;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.Game {
    public class GameEnded : MonoBehaviour {
        private void Awake() {
            var winnerNickname = GameObject.Find("WinnerNickname").GetComponent<TextMeshPro>();
            winnerNickname.text = GameManager.Instance.WinnerNickname;
        }
    }
}