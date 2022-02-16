using System;
using System.Collections.Generic;
using System.Net;
using Assets.Scripts.General;
using Assets.Scripts.Network.Common;
using Assets.Scripts.Utils.Messages;
using Assets.Scripts.Utils.Network.TCP;
using Google.Protobuf;

namespace Assets.Scripts.Network.Server {
    public class TcpServerManager : IServerCommunicatorForHost, IDisposable {
        private readonly TcpServerCommunicatorReceiverThread _tcpServerCommunicatorReceiverThread;
        private readonly TcpServerCommunicatorSenderThread _tcpServerCommunicatorSenderThread;

        public TcpServerManager(IPEndPoint localEndPoint) {
            Communicator = new QueueCommunicator();
            Clients = new SynchronizedCollection<Common.Client>();
            Server = new TcpServer(localEndPoint);
            _tcpServerCommunicatorSenderThread = new TcpServerCommunicatorSenderThread(this);
            _tcpServerCommunicatorSenderThread.Start();
            _tcpServerCommunicatorReceiverThread = new TcpServerCommunicatorReceiverThread(this);
            _tcpServerCommunicatorReceiverThread.Start();
        }

        public QueueCommunicator Communicator { get; }

        public TcpServer Server { get; }

        public SynchronizedCollection<Common.Client> Clients { get; }


        public void Dispose() {
            _tcpServerCommunicatorReceiverThread?.Stop();
            _tcpServerCommunicatorSenderThread?.Stop();
            Server?.Dispose();
        }

        public void HostConnect(string nickname) {
            var hostClient = new HostClient(new ClientDetails(nickname));
            GameManager.Instance.ClientId = hostClient.Id;
            Clients.Add(hostClient);
        }

        public void AddMessageToReceive(MessageToReceive messageToReceive) {
            Communicator.AddMessageToReceive(messageToReceive);
        }

        public void SendByNickname(IMessage message, string nickname) {
            foreach (var client in Clients) {
                if (client.Details.Nickname == nickname) {
                    Communicator.Send(message, new List<IPEndPoint> {client.GetEndpoint()});
                }
            }
        }
    }
}