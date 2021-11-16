using System.Net;
using System.Net.Sockets;
using Assets.Scripts.Utils;
using UnityEngine;

namespace Assets.Scripts.Network.Server {
    public class UdpServerReceiverThread : BaseThread {
        private const int PollTimeoutMs = 1000;
        private readonly UdpServerCommunicator _udpServerCommunicator;

        public UdpServerReceiverThread(UdpServerCommunicator udpCommunicator) {
            _udpServerCommunicator = udpCommunicator;
        }

        protected override void RunThread() {
            while (ThreadShouldRun) {
                if (_udpServerCommunicator.Client.Sock.Poll(PollTimeoutMs, SelectMode.SelectRead)) {
                    var endPoint = new IPEndPoint(IPAddress.Any, 0);
                    var messageBytes = _udpServerCommunicator.Client.Receive(ref endPoint);
                    var message = MessagesHelpers.ConvertBytesToMessage(messageBytes);
                    if (!_udpServerCommunicator.Clients.Contains(endPoint)) {
                        _udpServerCommunicator.Clients.Add(endPoint);
                    }

                    Debug.Log("Got message, inserting to queue");
                    _udpServerCommunicator.MessagesQueue.Enqueue(new Message(endPoint, message));
                }
            }
        }
    }
}