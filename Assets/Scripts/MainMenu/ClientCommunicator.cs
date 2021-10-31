using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
public class ClientCommunicator : MonoBehaviour {
    Network.Utils.TcpClient _client;

    void Start() {
        _client = new Network.Utils.TcpClient(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 12345));
    }

    void Update() {

    }

    private void OnDestroy() {
        _client.Close();    
    }

    private void Ready() {
        // TODO: send ready message
    }
}
