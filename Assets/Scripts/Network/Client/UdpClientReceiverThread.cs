using System.Net;
using System.Net.Sockets;
using Assets.Scripts.General;
using Assets.Scripts.Utils;
using UnityEngine;

namespace Assets.Scripts.Network.Client {
    public class UdpClientReceiverThread : BaseThread {
        private readonly UdpClientCommunicator _udpClientCommunicator;

        public UdpClientReceiverThread(UdpClientCommunicator udpClientCommunicator) {
            _udpClientCommunicator = udpClientCommunicator;
        }

        protected override void RunThread() {
            while (ThreadShouldRun) {
                if (_udpClientCommunicator.Client.Sock.Poll(NetworkManager.Instance.PollTimeoutMs,
                    SelectMode.SelectRead)) {
                    var endPoint = new IPEndPoint(IPAddress.Any, 0);
                    var messageBytes = _udpClientCommunicator.Client.Receive(ref endPoint);
                    var message = MessagesHelpers.ConvertBytesToMessage(messageBytes);
                    Debug.Log("Got message, inserting to queue");
                    _udpClientCommunicator.MessagesQueue.Enqueue(new Message(endPoint, message));
                }
            }
        }
    }
}