using System;
using System.IO;
using UnityEngine;

namespace Assets.Scripts.General {
    /// <summary>
    ///     Singleton which register itself to the Unity logging system in order to write
    ///     logs to file.
    ///     This singleton must inherit MonoBehaviour in order to register Application.logMessageReceived
    /// </summary>
    public class LogManager : MonoBehaviour {
        public static LogManager Instance;
        private readonly string _filename = "evade.log";

        private void Awake() {
            if (Instance != null && Instance != this) {
                Destroy(gameObject);
            } else {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }

        private void OnEnable() {
            Application.logMessageReceived += Log;
            Debug.Log("Started logging...");
        }

        private void OnDisable() {
            Application.logMessageReceived -= Log;
        }

        public void Log(string logString, string stackTrace, LogType type) {
            try {
                var currentTime = DateTime.Now.ToString(GameConsts.LogDatetimeFormat);
                File.AppendAllText(_filename, $"{currentTime} [{type}] {logString}\n");
                if (!stackTrace.Equals("")) {
                    File.AppendAllText(_filename, $"{stackTrace}\n");
                }
            } catch {
                // ignored
            }
        }
    }
}