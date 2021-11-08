using System.Collections.Generic;
using System.Net.Sockets;
using Evade.Utils;
using Evade.Utils.Network;
using Google.Protobuf.WellKnownTypes;
using UnityEngine;

namespace Evade.MainMenu {
    public class TcpServerCommunicator : BaseThread {

        public SynchronizedCollection<Client> Clients { get; private set; }
        private TcpServer _server;
        const int PORT = 12345;
        const int SELECT_TIMEOUT_MS = 1000;


        public TcpServerCommunicator(string ipAddress, int listeningPort) {
            Clients = new SynchronizedCollection<Client>();
            _server = new TcpServer(ipAddress, listeningPort);
        }

        private List<Socket> GetSocketListFromClients() {
            List<Socket> clientSocketList = new List<Socket>();
            foreach (Client client in Clients) {
                clientSocketList.Add(client.TcpClient.Sock);
            }
            return clientSocketList;
        }

        public void SendToAllClients(Google.Protobuf.IMessage message) {
            byte[] messageBytes = MessagesHelpers.ConvertMessageToBytes(message);
            foreach (Client client in Clients) {
                client.TcpClient.Send(messageBytes);
            }
        }

        private void HandleNewClient() {
            Debug.Log("New client connected");
            Clients.Add(new Client(_server.Accept()));
        }

        private Client FindClientBySocket(Socket sock) {
            foreach (Client client in Clients) {
                if (client.TcpClient.Sock == sock) {
                    return client;
                }
            }
            throw new SocketException();
        }

        private void HandleClientMessage(Socket sock) {
            Client client = FindClientBySocket(sock);
            try {
                byte[] messageByes = client.TcpClient.Recieve();
                Any message = MessagesHelpers.ConvertBytesToMessage(messageByes);
                if (message.Is(ClientReadyMessage.Descriptor)) {
                    Debug.Log($"Client {client.Details.Nickname} sent Ready message");
                    client.Details.ToggleReady();
                } else if (message.Is(ClientDetailsMessage.Descriptor)) {
                    Debug.Log($"Client {client.Details.Nickname} sent initilize details message");
                    ClientDetailsMessage details = message.Unpack<ClientDetailsMessage>();
                    client.Details.Nickname = details.Nickname;
                } else {
                    Debug.Log("Unknown message type");
                    return;
                }
            } catch (SocketClosedException) {
                Clients.Remove(client);
            }
            StateUpdateToAllClients();
        }

        private void StateUpdateToAllClients() {
            MainMenuStateMessage mainMenuMessage = new MainMenuStateMessage();
            foreach (Client client in Clients) {
                ClientDetailsMessage detailsMessage = new ClientDetailsMessage();
                detailsMessage.Nickname = client.Details.Nickname;
                detailsMessage.IsReady = client.Details.IsReady;
                mainMenuMessage.ClientsDetails.Add(detailsMessage);
            }
            SendToAllClients(mainMenuMessage);
        }

        private void HandleReadySockets(List<Socket> readySocket) {
            foreach (Socket sock in readySocket) {
                if (sock == _server.Sock) {
                    HandleNewClient();
                } else {
                    HandleClientMessage(sock);
                }
            }
        }

        protected override void RunThread() {
            while (ThreadShouldRun) {
                List<Socket> checkReadSockets = GetSocketListFromClients();
                checkReadSockets.Add(_server.Sock);
                Socket.Select(checkReadSockets, null, null, SELECT_TIMEOUT_MS);
                HandleReadySockets(checkReadSockets);
            }
        }
    }
    
    public class Client {
        public ClientDetails Details { get; private set; }
        public Utils.Network.TcpClient TcpClient { get; private set; }

        public Client(Utils.Network.TcpClient tcpClient) {
            TcpClient = tcpClient;
            Details = new ClientDetails();
        }

        public override string ToString() {
            return $"IP: {TcpClient} | {Details}";
        }
    }

}