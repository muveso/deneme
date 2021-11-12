using System;
using System.Collections.Concurrent;
using System.Net.Sockets;
using Evade.Utils;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using UnityEngine;
using UdpClient = Evade.Utils.Network.UdpClient;

namespace Evade {
    public class UdpCommunicator : BaseThread, IDisposable {
        private const int POLL_TIMEOUT_MS = 1000;
        protected UdpClient _client;

        public UdpCommunicator(string ipAddress, int port) {
            MessagesQueue = new ConcurrentQueue<Any>();
            _client = new UdpClient(ipAddress, port);
        }

        public UdpCommunicator(int listenPort) {
            MessagesQueue = new ConcurrentQueue<Any>();
            _client = new UdpClient(listenPort);
        }

        public ConcurrentQueue<Any> MessagesQueue { get; }

        public void Dispose() {
            Stop();
            _client.Dispose();
        }

        public void Send(IMessage message) {
            _client.Send(MessagesHelpers.ConvertMessageToBytes(message));
        }

        public Any TryGetMessageFromQueue() {
            Any message;
            MessagesQueue.TryDequeue(out message);
            return message;
        }

        protected override void RunThread() {
            while (ThreadShouldRun) {
                if (_client.Sock.Poll(POLL_TIMEOUT_MS, SelectMode.SelectRead)) {
                    var messageBytes = _client.Recieve();
                    var message = MessagesHelpers.ConvertBytesToMessage(messageBytes);
                    Debug.Log("Got message, inserting to queue");
                    MessagesQueue.Enqueue(message);
                }
            }
        }
    }
}