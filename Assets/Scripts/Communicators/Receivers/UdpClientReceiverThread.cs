using System.Net;
using System.Net.Sockets;
using Evade.Utils;
using UnityEngine;

namespace Evade.Communicators {
    public class UdpClientReceiverThread : BaseThread {
        private const int PollTimeoutMs = 1000;
        private readonly UdpClientCommunicator _udpCommunicator;

        public UdpClientReceiverThread(UdpClientCommunicator udpCommunicator) {
            _udpCommunicator = udpCommunicator;
        }

        protected override void RunThread() {
            while (ThreadShouldRun) {
                if (_udpCommunicator.Client.Sock.Poll(PollTimeoutMs, SelectMode.SelectRead)) {
                    var endPoint = new IPEndPoint(IPAddress.Any, 0);
                    var messageBytes = _udpCommunicator.Client.Receive(ref endPoint);
                    var message = MessagesHelpers.ConvertBytesToMessage(messageBytes);
                    Debug.Log("Got message, inserting to queue");
                    _udpCommunicator.MessagesQueue.Enqueue(new Message(endPoint, message));
                }
            }
        }
    }
}