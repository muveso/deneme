using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using Utils.Network;
using Utils;
using Google.Protobuf;

public class Client {
    public Utils.Network.TcpClient TcpClient { get; private set; }
    private bool _isReady = false;
    public Client(Utils.Network.TcpClient tcpClient) {
        TcpClient = tcpClient;
    }
    public bool IsReady => _isReady;
    public void ToggleReady() => _isReady = !_isReady;
}

public class HostCommunicatorThread : Utils.BaseThread {
    
    public SynchronizedCollection<Client> Clients { get; private set; }
    private TcpServer _server;
    const int PORT = 12345;
    const int SELECT_TIMEOUT_MS = 1000;


    public HostCommunicatorThread(string ipAddress, int listeningPort) {
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
            BaseMessage baseMessage = MessagesHelpers.GetBaseMessage(messageByes);
            if (MessagesHelpers.IsMessageTypeOf(baseMessage, ClientReadyMessage.Descriptor)) {
                client.ToggleReady();
            } else {
                Debug.Log("Unknown message type");
            }
        } catch (Utils.Network.SocketClosedException) {
            Clients.Remove(client);
        }
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
