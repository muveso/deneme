using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class HostCommunicator : MonoBehaviour, ICommunicator {
    
    SynchronizedCollection<Network.Utils.TcpClient> _clients;
    Network.Utils.TcpServer _server;
    Thread _tcpServerThread;
    const int PORT = 12345;
    const int SELECT_TIMEOUT_MS = 1000;

    void Start() {
        _clients = new SynchronizedCollection<Network.Utils.TcpClient>();
        _server = new Network.Utils.TcpServer(PORT);
        _tcpServerThread = new Thread(TcpServerLoop);
        _tcpServerThread.Start();
    }

    private void OnDestroy() {
        if (_tcpServerThread != null) {
            _tcpServerThread.Abort();
        }
    }

    private List<Socket> GetSocketListFromTcpClients() {
        List<Socket> clientSocketList = new List<Socket>();
        foreach (Network.Utils.TcpClient client in _clients) {
            clientSocketList.Add(client.Sock);
        }
        return clientSocketList;
    }

    private void HandleNewClient() {
        Debug.Log("New client connected");
        _clients.Add(_server.Accept());
    }

    private Network.Utils.TcpClient FindClientBySocket(Socket sock) {
        foreach (Network.Utils.TcpClient tcpClient in _clients) {
            if (tcpClient.Sock == sock) {
                return tcpClient;
            }
        }
        throw new SocketException();
    }

    private Network.Utils.TcpClient DeleteClientBySocket(Socket sock) {
        foreach (Network.Utils.TcpClient tcpClient in _clients) {
            if (tcpClient.Sock == sock) {
                return tcpClient;
            }
        }
        throw new SocketException();
    }

    private void HandleClientMessage(Socket sock) {
        Network.Utils.TcpClient tcpClient = FindClientBySocket(sock);
        try {
            byte[] messageByes = tcpClient.Recieve();
        } catch (Network.Utils.SocketClosedException) {
            _clients.Remove(tcpClient);
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

    private void TcpServerLoop() {
        while (true) {
            List<Socket> checkReadSockets = GetSocketListFromTcpClients();
            checkReadSockets.Add(_server.Sock);
            Socket.Select(checkReadSockets, null, null, SELECT_TIMEOUT_MS);
            HandleReadySockets(checkReadSockets);
        }
    }

    public SynchronizedCollection<Network.Utils.TcpClient> GetClientList() {
        return _clients;
    }
}
