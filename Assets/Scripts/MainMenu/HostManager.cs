using System;
using UnityEngine;

public class HostManager : ClientManager {
    private Server _server;
    const string DEFAULT_SERVER_IP_ADDRESS = "0.0.0.0";

    private void Start() {
        IPInputField.text = DEFAULT_SERVER_IP_ADDRESS;
        _nickname = "PanCHocK2";
    }

    protected override void InitializeCommunicator() {
        _clientCommunicator = new ClientCommunicator("127.0.0.1", int.Parse(PortInputField.text));
        _clientCommunicator.Start();
    }

    public override void OnClickConnect() {
        try {
            Debug.Log("Starting Server");
            _server = new Server(IPInputField.text, Int32.Parse(PortInputField.text));
            _server.Start();

            base.OnClickConnect();
        } catch (Exception e) {
            Debug.LogError(e.Message);
        }
    }

    protected override void OnDestroy() {
        Debug.Log("HostManager destroyed");
        if (_server != null && _server.IsAlive) {
            Debug.Log("Destrotying HostCommunicator Thread");
            _server.Stop();
        }
        base.OnDestroy();
    }

    public void OnClickStartGame() {
        if (AreAllClientsReady()) {
            Debug.Log("Starting Game");
        } else {
            Debug.Log("Not all clients ready");
        }
    }

    private bool AreAllClientsReady() {
        foreach (ClientDetails clientDetails in _clientCommunicator.Clients) {
            if (!clientDetails.IsReady) {
                return false;
            }
        }
        return true;
    }
}
