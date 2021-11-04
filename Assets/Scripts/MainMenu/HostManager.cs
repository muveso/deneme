using System;
using UnityEngine;
using UnityEngine.UI;

public class HostManager : MonoBehaviour {
    private HostCommunicatorThread _hostCommunicator;
    public InputField IPInputField;
    public InputField PortInputField;
    const int PLAYER_GAME_OBJECT_WIDTH = 600;
    const int PLAYER_GAME_OBJECT_HEIGHT = 90;
    const int PLAYER_GAME_OBJECT_FONT_SIZE = 30;
    const string DEFAULT_SERVER_IP_ADDRESS = "0.0.0.0";

    private void Start() {
        IPInputField.text = DEFAULT_SERVER_IP_ADDRESS;
    }

    void Update() {
        DestroyAllChildren();
        FillScrollViewWithClients();
    }

    public void OnClickStartHostServer() {
        Debug.Log("Starting HostCommunicator Thread");
        _hostCommunicator = new HostCommunicatorThread(IPInputField.text, Int32.Parse(PortInputField.text));
        _hostCommunicator.Start();
    }

    private void OnDestroy() {
        Debug.Log("HostManager destroyed");
        if (_hostCommunicator != null && _hostCommunicator.IsAlive) {
            Debug.Log("Destrotying HostCommunicator Thread");
            _hostCommunicator.Stop();
        }
    }

    private void DestroyAllChildren() {
        foreach (Transform child in transform) {
            GameObject.Destroy(child.gameObject);
        }
    }

    private void AddNewClientToList(Client client, int index) {
        GameObject newGameObject = new GameObject();
        Text myText = newGameObject.AddComponent<Text>();
        myText.text = $"{index}. {client.TcpClient} | Ready: {client.IsReady}";
        myText.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
        myText.fontSize = PLAYER_GAME_OBJECT_FONT_SIZE;
        myText.color = Color.black;
        newGameObject.transform.SetParent(transform);
        newGameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(PLAYER_GAME_OBJECT_WIDTH,
                                                                            PLAYER_GAME_OBJECT_HEIGHT);
    }

    void FillScrollViewWithClients() {
        if (_hostCommunicator != null) {
            int index = 1;
            foreach (Client client in _hostCommunicator.Clients) {
                AddNewClientToList(client, index);
                index++;
            }
        }
    }
}
