using System;
using System.Collections.Generic;
using Evade.Utils.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Evade.MainMenu {
    public abstract class AbstractClientManager : MonoBehaviour {
        protected List<ClientDetails> Clients;
        public InputField IPInputField;
        public InputField PortInputField;


        protected virtual void Awake() {
            Clients = new List<ClientDetails>();
        }

        protected virtual void Update() {
            if (HandleCommunicatorMessage()) {
                General.DestroyAllChildren(transform);
                FillScrollViewWithClients();
            }
        }

        protected abstract bool HandleCommunicatorMessage();

        private void FillScrollViewWithClients() {
            var index = 1;
            foreach (var client in Clients) {
                AddNewClientToList(client, index);
                index++;
            }
        }

        private void AddNewClientToList(ClientDetails client, int index) {
            var newGameObject = ScrollView.CreateNewTextItemForScrollView(client,
                index);
            newGameObject.transform.SetParent(transform);
        }

        public virtual void OnClickConnect() {
            try {
                ConnectLogic();
                EventSystem.current.currentSelectedGameObject.GetComponent<Button>().interactable = false;
            } catch (Exception e) {
                Debug.LogError(e.Message);
            }
        }

        protected abstract void ConnectLogic();

        public abstract void OnClickReady();
    }
}