using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Evade.MainMenu {
    public class ClientManager : MonoBehaviour {
        public InputField IPInputField;
        public InputField PortInputField;
        protected string _nickname = "PanCHocK";

        protected virtual void Update() {
            Evade.Utils.UI.General.DestroyAllChildren(transform);
            FillScrollViewWithClients();
        }

        private void FillScrollViewWithClients() {
            if (GameManager.Instance.TcpClientCommunicator != null) {
                int index = 1;
                foreach (ClientDetails client in GameManager.Instance.TcpClientCommunicator.Clients) {
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
            if (GameManager.Instance.TcpClientCommunicator == null) {
                Debug.LogError("ClientCommunicator is null");
                return;
            }
            ClientDetailsMessage clientDetailsMessage = new ClientDetailsMessage();
            clientDetailsMessage.Nickname = _nickname;
            GameManager.Instance.TcpClientCommunicator.Send(clientDetailsMessage);
        }

        protected virtual void InitializeCommunicator() {
            GameManager.Instance.TcpClientCommunicator = new TcpClientCommunicator(IPInputField.text, int.Parse(PortInputField.text));
            GameManager.Instance.TcpClientCommunicator.Start();
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
            if (GameManager.Instance.TcpClientCommunicator == null) {
                Debug.LogError("ClientCommunicator is null");
                return;
            }
            ClientReadyMessage clientReadyMessage = new ClientReadyMessage();
            GameManager.Instance.TcpClientCommunicator.Send(clientReadyMessage);
        }
    }
}