using System;
using Assets.Scripts.General;
using Assets.Scripts.Network.Host;
using Assets.Scripts.Network.Server;
using Assets.Scripts.Utils;
using Google.Protobuf.WellKnownTypes;
using UnityEngine;

namespace Assets.Scripts.Game {
    public class HostGame : MonoBehaviour {
        private void Awake() {
            NetworkManager.Instance.Communicators.UdpServerCommunicator =
                new UdpServerCommunicator(GameConsts.DefaultUdpServerPort);
            NetworkManager.Instance.Communicators.UdpClientCommunicator =
                new HostClientCommunicator(NetworkManager.Instance.Communicators.UdpServerCommunicator,
                    ClientGlobals.Nickname);
        }

        private void Update() {
            // All the game logic is here
            // Host does not need to get messages from server like the client because he is the server

            var message = NetworkManager.Instance.Communicators.UdpServerCommunicator.GetMessage();
            if (message != null) {
                Debug.Log("HostLobby got message");
                HandleMessage(message);
                SendGlobalStateToAllClients();
            }
        }

        private void SendGlobalStateToAllClients() {
            var globalState = GetGlobalState();
            NetworkManager.Instance.Communicators.UdpServerCommunicator.SendToAllClients(globalState);
        }

        private Any GetGlobalState() {
            throw new NotImplementedException();
        }

        private void HandleMessage(Message message) {
            // ServerUpdate()
        }
    }
}