using System;
using System.Collections.Generic;
using Evade.Communicators;
using Evade.Utils.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Evade.MainMenu {
    public class ClientManager : MonoBehaviour {
        protected IClientCommunicator ClientCommunicator;
        public InputField IPInputField;
        protected List<ClientDetails> Players;
        public InputField PortInputField;

        protected virtual void Awake() {
            Players = new List<ClientDetails>();
            ClientGlobals.Nickname = "PanCHocK";
        }

        public virtual void OnDestroy() {
            ClientCommunicator?.Dispose();
        }

        protected virtual void Update() {
            if (ClientCommunicator == null) {
                return;
            }

            HandleCommunicatorMessage();
            General.DestroyAllChildren(transform);
            FillScrollViewWithClients();
        }

        protected virtual List<ClientDetails> GetPlayers() {
            return Players;
        }

        private void HandleCommunicatorMessage() {
            var message = ClientCommunicator.GetMessage();
            if (message == null) {
                return;
            }

            if (message.ProtobufMessage.Is(MainMenuStateMessage.Descriptor)) {
                var mainMenuStateMessage = message.ProtobufMessage.Unpack<MainMenuStateMessage>();
                UpdateClients(mainMenuStateMessage);
            }
        }

        private void UpdateClients(MainMenuStateMessage mainMenuStateMessage) {
            var newPlayersList = new List<ClientDetails>();
            foreach (var clientDetailsMessage in mainMenuStateMessage.ClientsDetails) {
                newPlayersList.Add(new ClientDetails(clientDetailsMessage.Nickname, clientDetailsMessage.IsReady));
            }

            Players = newPlayersList;
        }

        private void FillScrollViewWithClients() {
            var index = 1;
            foreach (var player in GetPlayers()) {
                AddNewPlayerToList(player, index);
                index++;
            }
        }

        private void AddNewPlayerToList(ClientDetails player, int index) {
            var newGameObject = ScrollView.CreateNewTextItemForScrollView(player,
                index);
            newGameObject.transform.SetParent(transform);
        }

        public void SendClientDetails() {
            if (ClientCommunicator == null) {
                Debug.LogError("ClientCommunicator is null");
                return;
            }

            var clientDetailsMessage = new ClientDetailsMessage {
                Nickname = ClientGlobals.Nickname
            };
            ClientCommunicator.Send(clientDetailsMessage);
        }

        protected virtual void InitializeCommunicator() {
            ClientCommunicator = new TcpClientCommunicator(IPInputField.text, int.Parse(PortInputField.text));
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
            if (ClientCommunicator == null) {
                Debug.LogError("ClientCommunicator is null");
                return;
            }

            var clientReadyMessage = new ClientReadyMessage();
            ClientCommunicator.Send(clientReadyMessage);
        }
    }
}