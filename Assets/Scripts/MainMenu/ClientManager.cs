using System;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utils;

public class ClientManager : MonoBehaviour {
    Utils.Network.TcpClient _client;
    public InputField IPInputField;
    public InputField PortInputField;

    public void OnClickConnect() {
        try {
            IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Parse(IPInputField.text),
                                                       int.Parse(PortInputField.text));
            _client = new Utils.Network.TcpClient(serverEndPoint);
            EventSystem.current.currentSelectedGameObject.GetComponent<Button>().interactable = false;
        } catch (Exception e) {
            Debug.LogError(e.Message);
        }
    }
    
    public void OnClickReady() {
        if (_client != null && _client.IsConnected) {
            ClientReadyMessage clientReadyMessage = new ClientReadyMessage();
            _client.Send(MessagesHelpers.WrapMessage(clientReadyMessage));
        } else {
            Debug.LogError("Client is not connected");
        }
    }
}
