using System;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utils;

public class ClientManager : MonoBehaviour {
    private ClientCommunicator _clientCommunicator;
    public InputField IPInputField;
    public InputField PortInputField;
    private string _nickname = "PanCHocK";
    
    private void Update() {
        Utils.UI.General.DestroyAllChildren(transform);
        FillScrollViewWithClients();    
    }

    private void FillScrollViewWithClients() {
        if (_clientCommunicator != null) {
            int index = 1;
            foreach (ClientDetails client in _clientCommunicator.Clients) {
                AddNewClientToList(client, index);
                index++;
            }
        }
    }

    private void AddNewClientToList(ClientDetails client, int index) {
        GameObject newGameObject = Utils.UI.ScrollView.CreateNewTextItemForScrollView(client, index);
        newGameObject.transform.SetParent(transform);
    }

    private void OnDestroy() {
        if (_clientCommunicator != null) {
            _clientCommunicator.Stop();
        }
    }

    public void OnClickConnect() {
        try {
            _clientCommunicator = new ClientCommunicator(IPInputField.text, int.Parse(PortInputField.text));
            _clientCommunicator.Start();
            SendClientDetails();
            EventSystem.current.currentSelectedGameObject.GetComponent<Button>().interactable = false;
        } catch (Exception e) {
            Debug.LogError(e.Message);
        }
    }
    
    public void SendClientDetails() {
        if (_clientCommunicator == null) {
            Debug.LogError("ClientCommunicator is null");
            return;
        }
        ClientDetailsMessage clientDetailsMessage = new ClientDetailsMessage();
        clientDetailsMessage.Nickname = _nickname;
        _clientCommunicator.Send(clientDetailsMessage);
    }

    public void OnClickReady() {
        if (_clientCommunicator == null) {
            Debug.LogError("ClientCommunicator is null");
            return;
        }
        ClientReadyMessage clientReadyMessage = new ClientReadyMessage();
        _clientCommunicator.Send(clientReadyMessage);
    }
}
