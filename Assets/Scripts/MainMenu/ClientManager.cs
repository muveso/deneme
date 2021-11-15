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
        public InputField PortInputField;
        protected TcpClientCommunicator TcpClientCommunicator;

        protected virtual void Awake() {
            Clients = new List<ClientDetails>();
            ClientGlobals.Nickname = "PanCHocK";
        }

        public virtual void OnDestroy() {
            TcpClientCommunicator?.Dispose();
        }

        protected virtual void Update() {
            if (HandleCommunicatorMessage()) {
                General.DestroyAllChildren(transform);
                FillScrollViewWithClients();
            }
        }

        private bool HandleCommunicatorMessage() {
            if (TcpClientCommunicator == null) {
                return false;
            }

            if (!TcpClientCommunicator.MessagesQueue.TryDequeue(out var message)) {
                return false;
            }

            if (message.ProtobufMessage.Is(MainMenuStateMessage.Descriptor)) {
                var mainMenuStateMessage = message.ProtobufMessage.Unpack<MainMenuStateMessage>();
                UpdateClients(mainMenuStateMessage);
            }

            return true;
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

            var clientDetailsMessage = new ClientDetailsMessage {
                Nickname = ClientGlobals.Nickname
            };
            TcpClientCommunicator.Send(clientDetailsMessage);
        }

        protected virtual void InitializeCommunicator() {
            TcpClientCommunicator = new TcpClientCommunicator(IPInputField.text, int.Parse(PortInputField.text));
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