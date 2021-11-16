using Assets.Scripts.General;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.MainMenu {
    public class MainMenuController : MonoBehaviour {
        private void Start() {
            Debug.Log("MainMenuController created");
        }

        public void OnClickClient() {
            Debug.Log("Client chosen");
            SceneManager.LoadScene("Client");
        }

        public void OnClickHost() {
            Debug.Log("Host chosen");
            SceneManager.LoadScene("Host");
        }

        public void OnClickGoBackToMainMenu() {
            NetworkManager.Instance.Reset();
            SceneManager.LoadScene("MainMenu");
        }
    }
}