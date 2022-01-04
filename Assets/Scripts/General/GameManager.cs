using System;
using System.Collections.Generic;
using System.Net;
using Assets.Scripts.Network.Common;
using UnityEngine;

namespace Assets.Scripts.General {
    public class GameManager : Singleton<GameManager> {
        public GameManager() {
            Reset();
        }

        public IPAddress ServerIpAddress { get; set; }
        public NetworkManagers NetworkManagers { get; set; }
        public bool IsHost { get; set; }
        public string Nickname { get; set; }
        public string ClientId { get; set; }
        public bool IsGameEnded { get; set; }
        public string WinnerNickname { get; set; }
        public Dictionary<string, Tuple<GameObject, Client>> ServerGameObjects { get; set; }

        public void Reset() {
            IsHost = true;
            ServerIpAddress = null;
            Nickname = null;
            ClientId = null;
            IsGameEnded = false;
            WinnerNickname = null;
            NetworkManagers?.Dispose();
            NetworkManagers = new NetworkManagers();
            ServerGameObjects = new Dictionary<string, Tuple<GameObject, Client>>();
        }
    }
}