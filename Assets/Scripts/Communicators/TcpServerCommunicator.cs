using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Evade.Utils;
using Evade.Utils.Network;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using UnityEngine;
using TcpClient = Evade.Utils.Network.TcpClient;

namespace Evade.Communicators {
    public class TcpServerCommunicator : BaseThread, IDisposable {
        private const int SelectTimeoutMs = 1000;
        private readonly TcpServer _server;


        public TcpServerCommunicator(string ipAddress, int listeningPort) {
            MessagesQueue = new ConcurrentQueue<Any>();
            Clients = new SynchronizedCollection<Client>();
            _server = new TcpServer(ipAddress, listeningPort);
        }

        public SynchronizedCollection<Client> Clients { get; }

        public ConcurrentQueue<Any> MessagesQueue { get; }

        public void Dispose() {
            Stop();
            _server.Dispose();
        }

        public List<IPEndPoint> GetEndpointListFromClients() {
            var endpointsList = new List<IPEndPoint>();
            foreach (var client in Clients) {
                endpointsList.Add(client.TcpClient.Sock.RemoteEndPoint as IPEndPoint);
            }

            return endpointsList;
        }

        private List<Socket> GetSocketListFromClients() {
            var clientSocketList = new List<Socket>();
            foreach (var client in Clients) {
                clientSocketList.Add(client.TcpClient.Sock);
            }

            return clientSocketList;
        }

        public void SendToAllClients(IMessage message) {
            var messageBytes = MessagesHelpers.ConvertMessageToBytes(message);
            foreach (var client in Clients) {
                client.TcpClient.Send(messageBytes);
            }
        }

        private void HandleNewClient() {
            Debug.Log("New client connected");
            Clients.Add(new Client(_server.Accept()));
        }

        private Client FindClientBySocket(Socket sock) {
            foreach (var client in Clients) {
                if (client.TcpClient.Sock == sock) {
                    return client;
                }
            }

            throw new SocketException();
        }

        private void HandleClientMessage(Socket sock) {
            var client = FindClientBySocket(sock);
            try {
                var messageByes = client.TcpClient.Recieve();
                var message = MessagesHelpers.ConvertBytesToMessage(messageByes);
                if (message.Is(ClientReadyMessage.Descriptor)) {
                    Debug.Log($"Client {client.Details.Nickname} sent Ready message");
                    client.Details.ToggleReady();
                } else if (message.Is(ClientDetailsMessage.Descriptor)) {
                    Debug.Log($"Client {client.Details.Nickname} sent initilize details message");
                    var details = message.Unpack<ClientDetailsMessage>();
                    client.Details.Nickname = details.Nickname;
                } else {
                    MessagesQueue.Enqueue(message);
                    return;
                }
            } catch (SocketClosedException) {
                Clients.Remove(client);
            }

            StateUpdateToAllClients();
        }

        private void StateUpdateToAllClients() {
            var mainMenuMessage = new MainMenuStateMessage();
            foreach (var client in Clients) {
                var detailsMessage = new ClientDetailsMessage {
                    Nickname = client.Details.Nickname,
                    IsReady = client.Details.IsReady
                };
                mainMenuMessage.ClientsDetails.Add(detailsMessage);
            }

            SendToAllClients(mainMenuMessage);
        }

        private void HandleReadySockets(List<Socket> readySocket) {
            foreach (var sock in readySocket) {
                if (sock == _server.Sock) {
                    HandleNewClient();
                } else {
                    HandleClientMessage(sock);
                }
            }
        }

        protected override void RunThread() {
            while (ThreadShouldRun) {
                var checkReadSockets = GetSocketListFromClients();
                checkReadSockets.Add(_server.Sock);
                Socket.Select(checkReadSockets, null, null, SelectTimeoutMs);
                HandleReadySockets(checkReadSockets);
            }
        }
    }

    public class Client {
        public Client(TcpClient tcpClient) {
            TcpClient = tcpClient;
            Details = new ClientDetails();
        }

        public ClientDetails Details { get; }
        public TcpClient TcpClient { get; }

        public override string ToString() {
            return $"IP: {TcpClient} | {Details}";
        }
    }
}