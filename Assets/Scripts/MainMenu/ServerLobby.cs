using System;
using System.Net;
using Assets.Scripts.General;
using Assets.Scripts.Network.Common;
using Assets.Scripts.Network.Server;
using Assets.Scripts.Utils.Messages;
using UnityEngine;
using UnityEngine.SceneManagement;
using Debug = System.Diagnostics.Debug;

namespace Assets.Scripts.MainMenu {
    public class ServerLobby : MonoBehaviour {
        public void StartServer(string ipAddress, string port) {
            GameManager.Instance.NetworkManagers.TcpServerManager =
                new TcpServerManager(new IPEndPoint(IPAddress.Parse(ipAddress),
                    int.Parse(port)));
        }
        
        private void Update() {
            if (GameManager.Instance.NetworkManagers.TcpServerManager == null) {
                return;
            }

            var message = GameManager.Instance.NetworkManagers.TcpServerManager.Communicator.Receive();
            if (message != null) {
                HandleMessage(message);
            }
        }

        private void HandleMessage(MessageToReceive messageToReceive) {
            var protobufMessage = messageToReceive.AnyMessage;
            var client = FindClientByIPEndpoint(messageToReceive.IPEndpoint);
            if (protobufMessage.Is(ClientReadyMessage.Descriptor)) {
                UnityEngine.Debug.Log($"Client {client.Details.Nickname} sent Ready messageToReceive");
                client.Details.ToggleReady();
                TryToStartGame();
            } else if (protobufMessage.Is(ClientDetailsMessage.Descriptor)) {
                UnityEngine.Debug.Log($"Client {client.Details.Nickname} sent initilize details messageToReceive");
                var details = protobufMessage.Unpack<ClientDetailsMessage>();
                client.Details.Nickname = details.Nickname;
            } else if (protobufMessage.Is(ClientDisconnectedMessage.Descriptor)) {
                UnityEngine.Debug.Log("Client disconnected");
            }

            StateUpdateToAllClients();
        }

        private Client FindClientByIPEndpoint(IPEndPoint messageIpEndpoint) {
            foreach (var client in GameManager.Instance.NetworkManagers.TcpServerManager.Clients) {
                if (Equals(client.GetEndpoint(), messageIpEndpoint)) {
                    return client;
                }
            }

            return null;
        }

        private void TryToStartGame() {
            if (AreAllPlayersReady()) {
                UnityEngine.Debug.Log("Starting Game");
                GameManager.Instance.NetworkManagers.TcpServerManager.Communicator.Send(new StartGameMessage());
                SceneManager.LoadScene("Game");
                Destroy(this);
            } else {
                UnityEngine.Debug.Log("Not all clients ready");
            }
        }

        private bool AreAllPlayersReady() {
            foreach (var client in GameManager.Instance.NetworkManagers.TcpServerManager
                         .Clients) {
                if (!client.Details.IsReady) {
                    return false;
                }
            }

            return true;
        }

        private void StateUpdateToAllClients() {
            var mainMenuMessage = new MainMenuStateMessage();
            foreach (var client in GameManager.Instance.NetworkManagers.TcpServerManager.Clients) {
                var detailsMessage = new ClientDetailsMessage {
                    Nickname = client.Details.Nickname,
                    IsReady = client.Details.IsReady
                };
                mainMenuMessage.ClientsDetails.Add(detailsMessage);
            }

            GameManager.Instance.NetworkManagers.TcpServerManager.Communicator.Send(mainMenuMessage);
        }
    }
}