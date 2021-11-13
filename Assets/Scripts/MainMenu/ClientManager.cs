using System;
using System.Collections.Generic;
using Evade.Communicators;
using Evade.Utils.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Evade.MainMenu {
    public class ClientManager : MonoBehaviour {
        protected List<ClientDetails> Clients;
        public InputField IPInputField;
        protected string Nickname = "PanCHocK";
        public InputField PortInputField;
        protected TcpClientCommunicator TcpClientCommunicator;

        public virtual void OnDestroy() {
            TcpClientCommunicator?.Dispose();
        }

        protected virtual void Start() {
            Clients = new List<ClientDetails>();
        }

        protected virtual void Update() {
            HandleCommunicatorMessage();
            General.DestroyAllChildren(transform);
            FillScrollViewWithClients();
        }

        private void HandleCommunicatorMessage() {
            if (!TcpClientCommunicator.MessagesQueue.TryDequeue(out var message)) {
                return;
            }

            if (message.Is(MainMenuStateMessage.Descriptor)) {
                var mainMenuStateMessage = message.Unpack<MainMenuStateMessage>();
                UpdateClients(mainMenuStateMessage);
            }
        }

        private void UpdateClients(MainMenuStateMessage mainMenuStateMessage) {
            var newClientsList = new List<ClientDetails>();
            foreach (var clientDetailsMessage in mainMenuStateMessage.ClientsDetails) {
                newClientsList.Add(new ClientDetails(clientDetailsMessage.Nickname, clientDetailsMessage.IsReady));
            }

            Clients = newClientsList;
        }

        private void FillScrollViewWithClients() {
            if (TcpClientCommunicator != null) {
                var index = 1;
                foreach (var client in Clients) {
                    AddNewClientToList(client, index);
                    index++;
                }
            }
        }

        private void AddNewClientToList(ClientDetails client, int index) {
            var newGameObject = ScrollView.CreateNewTextItemForScrollView(client,
                index);
            newGameObject.transform.SetParent(transform);
        }

        public void SendClientDetails() {
            if (TcpClientCommunicator == null) {
                Debug.LogError("ClientCommunicator is null");
                return;
            }

            var clientDetailsMessage = new ClientDetailsMessage();
            clientDetailsMessage.Nickname = Nickname;
            TcpClientCommunicator.Send(clientDetailsMessage);
        }

        protected virtual void InitializeCommunicator() {
            TcpClientCommunicator =
                new TcpClientCommunicator(IPInputField.text, int.Parse(PortInputField.text));
            TcpClientCommunicator.Start();
        }

        public virtual void OnClickConnect() {
            try {
                InitializeCommunicator();
                SendClientDetails();
                EventSystem.current.currentSelectedGameObject.GetComponent<Button>().interactable = false;
            } catch (Exception e) {
                Debug.LogError(e.Message);
            }
        }

        public void OnClickReady() {
            if (TcpClientCommunicator == null) {
                Debug.LogError("ClientCommunicator is null");
                return;
            }

            var clientReadyMessage = new ClientReadyMessage();
            TcpClientCommunicator.Send(clientReadyMessage);
        }
    }
}