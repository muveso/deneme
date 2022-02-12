using System;
using System.Net;
using Assets.Scripts.General;
using Assets.Scripts.MainMenu;
using Assets.Scripts.Network.Server;
using UnityEngine;

namespace MainMenu {
    public class Initializer : MonoBehaviour {
        private void Awake() {
            if (Application.isBatchMode) {
                Debug.Log("Running Headless server");
                var ipAddress = Environment.GetEnvironmentVariable("IP");
                var port = Environment.GetEnvironmentVariable("PORT");
                ServerLobby serverLobby = gameObject.AddComponent<ServerLobby>();
                serverLobby.StartServer(ipAddress, port);
            } else {
                Debug.Log("Running Host / Client...");
            }
        }
    }
}