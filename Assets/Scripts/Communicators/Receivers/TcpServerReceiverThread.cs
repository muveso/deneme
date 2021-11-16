using System.Collections.Generic;
using System.Net.Sockets;
using Evade.Utils;
using UnityEngine;

namespace Evade.Communicators {
    public class TcpServerReceiverThread : BaseThread {
        private const int SelectTimeoutMs = 1000;
        private readonly TcpServerCommunicator _tcpServerCommunicator;

        public TcpServerReceiverThread(TcpServerCommunicator tcpServerCommunicator) {
            _tcpServerCommunicator = tcpServerCommunicator;
        }

        protected override void RunThread() {
            while (ThreadShouldRun) {
                var checkReadSockets = GetSocketListFromClients();
                checkReadSockets.Add(_tcpServerCommunicator.Server.Sock);
                Socket.Select(checkReadSockets, null, null, SelectTimeoutMs);
                HandleReadySockets(checkReadSockets);
            }
        }

        private void HandleReadySockets(List<Socket> readySocket) {
            foreach (var sock in readySocket) {
                if (sock == _tcpServerCommunicator.Server.Sock) {
                    HandleNewClient();
                } else {
                    var client = FindClientBySocket(sock);
                    var messageByes = client.Receive();
                    var message = MessagesHelpers.ConvertBytesToMessage(messageByes);
                    _tcpServerCommunicator.MessagesQueue.Enqueue(
                        new Message(client.GetEndpoint(), message));
                }
            }
        }

        private List<Socket> GetSocketListFromClients() {
            var clientSocketList = new List<Socket>();
            foreach (var client in _tcpServerCommunicator.Clients) {
                if (HostClient.IsHostClient(client)) {
                    continue;
                }

                clientSocketList.Add(client.GetSock());
            }

            return clientSocketList;
        }

        private void HandleNewClient() {
            Debug.Log("New client connected");
            _tcpServerCommunicator.Clients.Add(new NetworkTcpClient(_tcpServerCommunicator.Server.Accept()));
        }


        private Client FindClientBySocket(Socket sock) {
            foreach (var client in _tcpServerCommunicator.Clients) {
                if (client.GetSock() == sock) {
                    return client;
                }
            }

            throw new SocketException();
        }
    }
}