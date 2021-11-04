using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using Utils.Network;

public class HostCommunicatorThread : Utils.BaseThread {
    
    public SynchronizedCollection<Utils.Network.TcpClient> Clients { get; private set; }
    private TcpServer _server;
    const int PORT = 12345;
    const int SELECT_TIMEOUT_MS = 1000;


    public HostCommunicatorThread(string ipAddress, int listeningPort) {
        Clients = new SynchronizedCollection<Utils.Network.TcpClient>();
        _server = new TcpServer(ipAddress, listeningPort);
    }

    private List<Socket> GetSocketListFromTcpClients() {
        List<Socket> clientSocketList = new List<Socket>();
        foreach (Utils.Network.TcpClient client in Clients) {
            clientSocketList.Add(client.Sock);
        }
        return clientSocketList;
    }

    private void HandleNewClient() {
        Debug.Log("New client connected");
        Clients.Add(_server.Accept());
    }

    private Utils.Network.TcpClient FindClientBySocket(Socket sock) {
        foreach (Utils.Network.TcpClient tcpClient in Clients) {
            if (tcpClient.Sock == sock) {
                return tcpClient;
            }
        }
        throw new SocketException();
    }

    private Utils.Network.TcpClient DeleteClientBySocket(Socket sock) {
        foreach (Utils.Network.TcpClient tcpClient in Clients) {
            if (tcpClient.Sock == sock) {
                return tcpClient;
            }
        }
        throw new SocketException();
    }

    private void HandleClientMessage(Socket sock) {
        Utils.Network.TcpClient tcpClient = FindClientBySocket(sock);
        try {
            byte[] messageByes = tcpClient.Recieve();
        } catch (Utils.Network.SocketClosedException) {
            Clients.Remove(tcpClient);
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
        while (ShouldRun()) {
            List<Socket> checkReadSockets = GetSocketListFromTcpClients();
            checkReadSockets.Add(_server.Sock);
            Socket.Select(checkReadSockets, null, null, SELECT_TIMEOUT_MS);
            HandleReadySockets(checkReadSockets);
        }
    }
}
