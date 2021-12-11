using System;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

namespace Assets.Scripts.General {
    public class GameManager {
        private static readonly object Padlock = new();
        private static GameManager _instance;

        private GameManager() {
            Reset();
        }

        public IPAddress ServerIpAddress { get; set; }
        public NetworkManagers NetworkManagers { get; set; }
        public bool IsHost { get; set; }
        public string Nickname { get; set; }
        public string ClientId { get; set; }
        public Dictionary<string, Tuple<GameObject, string>> ServerGameObjects { get; set; }

        public static GameManager Instance {
            get {
                // There is no reason to lock if the instance is already initialized
                if (_instance == null) {
                    // Lock to make sure that only one thread will create instance
                    lock (Padlock) {
                        if (_instance == null) {
                            _instance = new GameManager();
                        }
                    }
                }

                return _instance;
            }
        }

        // Manual reset for singleton
        public void Reset() {
            IsHost = false;
            ServerIpAddress = null;
            NetworkManagers?.Dispose();
            NetworkManagers = new NetworkManagers();
            ServerGameObjects = new Dictionary<string, Tuple<GameObject, string>>();
        }
    }
}