using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HostManager : MonoBehaviour {
    private HostCommunicatorThread _hostCommunicator;
    public Text IpText;
    public Text PortText;
    const int PLAYER_GAME_OBJECT_WIDTH = 600;
    const int PLAYER_GAME_OBJECT_HEIGHT = 90;
    const int PLAYER_GAME_OBJECT_FONT_SIZE = 50;
    
    private void Start() {
        IpText.text = "0.0.0.0";
    }

    void Update() {
        DestroyAllChildren();
        FillScrollViewWithClients();
    }

    public void OnClickStartHostServer() {
        _hostCommunicator = new HostCommunicatorThread(IpText.text, Int32.Parse(PortText.text));
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

    private void AddNewClientToList(Utils.Network.TcpClient client, int index) {
        GameObject newGameObject = new GameObject();
        Text myText = newGameObject.AddComponent<Text>();
        myText.text = $"{index}. {client}";
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
            foreach (var client in _hostCommunicator.Clients) {
                AddNewClientToList(client, index);
                index++;
            }
        }
    }
}
