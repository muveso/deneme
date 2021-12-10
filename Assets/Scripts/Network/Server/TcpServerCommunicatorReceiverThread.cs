using System.Collections.Generic;
using System.Net.Sockets;
using Assets.Scripts.Network.Common;
using Assets.Scripts.Utils;
using Assets.Scripts.Utils.Messages;
using Assets.Scripts.Utils.Network;
using UnityEngine;

namespace Assets.Scripts.Network.Server {
    public class TcpServerCommunicatorReceiverThread : BaseThread {
        private readonly TcpServerManager _tcpServerManager;

        public TcpServerCommunicatorReceiverThread(TcpServerManager tcpServerManager) {
            _tcpServerManager = tcpServerManager;
        }

        protected override void RunThread() {
            while (ThreadShouldRun) {
                var checkReadSockets = GetSocketListFromClients();
                checkReadSockets.Add(_tcpServerManager.Server.Sock);
                Socket.Select(checkReadSockets, null, null, 0);
                HandleReadySockets(checkReadSockets);
            }
        }

        private void HandleReadySockets(List<Socket> readySocket) {
            foreach (var sock in readySocket) {
                if (sock == _tcpServerManager.Server.Sock) {
                    HandleNewClient();
                } else {
                    var client = FindClientBySocket(sock);
                    try {
                        var messageByes = client.Receive();
                        var message = MessagesHelpers.ConvertBytesToMessage(messageByes);
                        _tcpServerManager.Communicator.AddMessageToReceive(
                            new MessageToReceive(client.GetEndpoint(), message));
                    } catch (SocketClosedException) {
                        _tcpServerManager.Clients.Remove(client);
                    }
                }
            }
        }

        private List<Socket> GetSocketListFromClients() {
            var clientSocketList = new List<Socket>();
            foreach (var client in _tcpServerManager.Clients) {
                if (HostClient.IsHostClient(client)) {
                    continue;
                }

                clientSocketList.Add(client.GetSock());
            }

            return clientSocketList;
        }

        private void HandleNewClient() {
            Debug.Log("New client connected");
            var newClient = new NetworkTcpClient(_tcpServerManager.Server.Accept());
            _tcpServerManager.Clients.Add(newClient);
            var setClientIdMessage = new SetClientIdMessage {
                ClientId = newClient.Id
            };
            newClient.Send(MessagesHelpers.ConvertMessageToBytes(setClientIdMessage));
        }


        private Common.Client FindClientBySocket(Socket sock) {
            foreach (var client in _tcpServerManager.Clients) {
                if (client.GetSock() == sock) {
                    return client;
                }
            }

            throw new SocketException();
        }
    }
}