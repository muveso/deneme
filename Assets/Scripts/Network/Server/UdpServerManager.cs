using System;
using System.Collections.Generic;
using System.Net;
using Assets.Scripts.Network.Client;
using Assets.Scripts.Network.Common;
using Assets.Scripts.Utils.Messages;
using Assets.Scripts.Utils.Network.UDP;

namespace Assets.Scripts.Network.Server {
    public class
        UdpServerManager : IServerCommunicatorForHost, IDisposable {
        private readonly UdpServerCommunicatorReceiverThread _udpServerCommunicatorReceiverThread;
        private readonly UdpServerCommunicatorSenderThread _udpServerCommunicatorSenderThread;

        public UdpServerManager(int listeningPort) {
            Communicator = new QueueCommunicator();
            Client = new UdpClientMessageBasedClient(new UdpClient(listeningPort));
            Clients = new SynchronizedCollection<IPEndPoint>();
            _udpServerCommunicatorSenderThread = new UdpServerCommunicatorSenderThread(this);
            _udpServerCommunicatorSenderThread.Start();
            _udpServerCommunicatorReceiverThread = new UdpServerCommunicatorReceiverThread(this);
            _udpServerCommunicatorReceiverThread.Start();
        }

        public QueueCommunicator Communicator { get; }

        public UdpClientMessageBasedClient Client { get; }
        public SynchronizedCollection<IPEndPoint> Clients { get; }

        public void Dispose() {
            _udpServerCommunicatorSenderThread?.Stop();
            _udpServerCommunicatorReceiverThread?.Stop();
            Client?.Dispose();
        }

        public void HostConnect(string nickname) { }

        public void AddMessageToReceive(MessageToReceive messageToReceive) {
            Communicator.AddMessageToReceive(messageToReceive);
        }
    }
}