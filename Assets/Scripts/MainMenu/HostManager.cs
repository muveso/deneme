using System;
using UnityEngine;
using UnityEngine.UI;

public class HostManager : MonoBehaviour {
    private Server _server;
    private ClientCommunicator _hostCommunicator;
    public InputField IPInputField;
    public InputField PortInputField;
    const string DEFAULT_SERVER_IP_ADDRESS = "0.0.0.0";

    private void Start() {
        IPInputField.text = DEFAULT_SERVER_IP_ADDRESS;
    }

    void Update() {
        Utils.UI.General.DestroyAllChildren(transform);
        FillScrollViewWithClients();
    }

    public void OnClickStartHostServer() {
        Debug.Log("Starting Server");
        _server = new Server(IPInputField.text, Int32.Parse(PortInputField.text));
        _server.Start();

        Debug.Log("Host connect");
        _hostCommunicator = new ClientCommunicator("127.0.0.1", Int32.Parse(PortInputField.text));
        _hostCommunicator.Start();
        SendClientDetails();
    }

    public void SendClientDetails() {
        if (_hostCommunicator == null) {
            Debug.LogError("ClientCommunicator is null");
            return;
        }
        ClientDetailsMessage clientDetailsMessage = new ClientDetailsMessage();
        clientDetailsMessage.Nickname = "AAA";
        _hostCommunicator.Send(clientDetailsMessage);
    }

    private void OnDestroy() {
        Debug.Log("HostManager destroyed");
        if (_server != null && _server.IsAlive) {
            Debug.Log("Destrotying HostCommunicator Thread");
            _server.Stop();
        }
    }

    private void AddNewClientToList(Client client, int index) {
        GameObject newGameObject = Utils.UI.ScrollView.CreateNewTextItemForScrollView(client, index);
        newGameObject.transform.SetParent(transform);
    }

    void FillScrollViewWithClients() {
        if (_server != null) {
            int index = 1;
            foreach (Client client in _server.Clients) {
                if (client.Details.Nickname != null) {
                    AddNewClientToList(client, index);
                    index++;
                }
            }
        }
    }
}
