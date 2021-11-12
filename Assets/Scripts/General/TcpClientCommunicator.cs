using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Evade.Utils;
using Google.Protobuf;
using UnityEngine;
using TcpClient = Evade.Utils.Network.TcpClient;

namespace Evade {
    public class TcpClientCommunicator : BaseThread, IDisposable {
        private const int POLL_TIMEOUT_MS = 1000;
        private readonly TcpClient _client;

        public TcpClientCommunicator(string ipAddress, int port) {
            var serverEndPoint = new IPEndPoint(IPAddress.Parse(ipAddress), port);
            Clients = new SynchronizedCollection<ClientDetails>();
            _client = new TcpClient(serverEndPoint);
        }

        public SynchronizedCollection<ClientDetails> Clients { get; private set; }

        public void Dispose() {
            Stop();
            _client.Dispose();
        }

        public IPEndPoint GetRemoteEndpoint() {
            return _client.Sock.RemoteEndPoint as IPEndPoint;
        }

        public void Send(IMessage message) {
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
            var messageBytes = _client.Recieve();
            var message = MessagesHelpers.ConvertBytesToMessage(messageBytes);
            if (message.Is(MainMenuStateMessage.Descriptor)) {
                var mainMenuStateMessage = message.Unpack<MainMenuStateMessage>();
                UpdateClients(mainMenuStateMessage);
            } else {
                Debug.LogError("Unknown message type");
            }
        }

        private void UpdateClients(MainMenuStateMessage mainMenuStateMessage) {
            var newClientsList = new SynchronizedCollection<ClientDetails>();
            foreach (var clientDetailsMessage in mainMenuStateMessage.ClientsDetails) {
                newClientsList.Add(new ClientDetails(clientDetailsMessage.Nickname, clientDetailsMessage.IsReady));
            }

            Clients = newClientsList;
        }
    }
}