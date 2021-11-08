using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Evade.Utils;
namespace Evade.MainMenu {
    public class ClientManager : MonoBehaviour {
        public InputField IPInputField;
        public InputField PortInputField;
        protected TcpClientCommunicator _clientCommunicator;
        protected string _nickname = "PanCHocK";

        protected virtual void Update() {
            Evade.Utils.UI.General.DestroyAllChildren(transform);
            FillScrollViewWithClients();
        }

        protected virtual void OnDestroy() {
            if (_clientCommunicator != null) {
                _clientCommunicator.Stop();
            }
        }

        private void FillScrollViewWithClients() {
            if (_clientCommunicator != null) {
                int index = 1;
                foreach (ClientDetails client in _clientCommunicator.Clients) {
                    AddNewClientToList(client, index);
                    index++;
                }
            }
        }

        private void AddNewClientToList(ClientDetails client, int index) {
            GameObject newGameObject = Utils.UI.ScrollView.CreateNewTextItemForScrollView(client,
                index);
            newGameObject.transform.SetParent(transform);
        }

        public void SendClientDetails() {
            if (_clientCommunicator == null) {
                Debug.LogError("ClientCommunicator is null");
                return;
            }
            ClientDetailsMessage clientDetailsMessage = new ClientDetailsMessage();
            clientDetailsMessage.Nickname = _nickname;
            _clientCommunicator.Send(clientDetailsMessage);
        }

        protected virtual void InitializeCommunicator() {
            _clientCommunicator = new TcpClientCommunicator(IPInputField.text, int.Parse(PortInputField.text));
            _clientCommunicator.Start();
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
            if (_clientCommunicator == null) {
                Debug.LogError("ClientCommunicator is null");
                return;
            }
            ClientReadyMessage clientReadyMessage = new ClientReadyMessage();
            _clientCommunicator.Send(clientReadyMessage);
        }
    }
}