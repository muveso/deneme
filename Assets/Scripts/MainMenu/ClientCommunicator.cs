using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class ClientCommunicator : MonoBehaviour {
    SynchronizedCollection<Utils.Network.TcpClient> _clients;
    Utils.Network.TcpClient _client;

    void Start() {
        _client = new Utils.Network.TcpClient(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 12345));
    }

    void Update() {

    }

    public void OnClickConnect() {
    }

    private void OnDestroy() {
    }

    private void Ready() {
        // TODO: send ready message
    }
}
