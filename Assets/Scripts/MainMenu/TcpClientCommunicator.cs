using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using Google.Protobuf.WellKnownTypes;
using System.Net;
using Evade.Utils;

namespace Evade.MainMenu {
    public class TcpClientCommunicator : BaseThread {

        public SynchronizedCollection<ClientDetails> Clients { get; private set; }
        private Utils.Network.TcpClient _client;
        const int POLL_TIMEOUT_MS = 1000;

        public TcpClientCommunicator(string ipAddress, int port) {
            IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Parse(ipAddress), port);
            Clients = new SynchronizedCollection<ClientDetails>();
            _client = new Utils.Network.TcpClient(serverEndPoint);
        }

        public void Send(Google.Protobuf.IMessage message) {
            _client.Send(MessagesHelpers.ConvertMessageToBytes(message));
        }

        protected override void RunThread() {
            while (ThreadShouldRun) {
                if (_client.Sock.Poll(POLL_TIMEOUT_MS, SelectMode.SelectRead)) {
                    HandleMessage();
                }
            }
        }

        private void HandleMessage() {
            byte[] messageBytes = _client.Recieve();
            Any message = MessagesHelpers.ConvertBytesToMessage(messageBytes);
            if (message.Is(MainMenuStateMessage.Descriptor)) {
                MainMenuStateMessage mainMenuStateMessage = message.Unpack<MainMenuStateMessage>();
                UpdateClients(mainMenuStateMessage);
            } else {
                Debug.LogError("Unknown message type");
            }
        }

        private void UpdateClients(MainMenuStateMessage mainMenuStateMessage) {
            SynchronizedCollection<ClientDetails> newClientsList = new SynchronizedCollection<ClientDetails>();
            foreach (ClientDetailsMessage clientDetailsMessage in mainMenuStateMessage.ClientsDetails) {
                newClientsList.Add(new ClientDetails(clientDetailsMessage.Nickname, clientDetailsMessage.IsReady));
            }
            Clients = newClientsList;
        }
    }
}