using System;
using System.Net;
using System.Collections.Concurrent;
using System.Net.Sockets;
using Evade.Utils;
using Google.Protobuf.WellKnownTypes;
using UnityEngine;

namespace Evade {
    public class UdpCommunicator : BaseThread, IDisposable {

        public ConcurrentQueue<Any> MessagesQueue { get; private set; }
        protected Utils.Network.UdpClient _client;
        const int POLL_TIMEOUT_MS = 1000;

        public UdpCommunicator(string ipAddress, int port) {
            MessagesQueue = new ConcurrentQueue<Any>();
            _client = new Utils.Network.UdpClient(ipAddress, port);
        }

        public UdpCommunicator(int listenPort) {
            MessagesQueue = new ConcurrentQueue<Any>();
            _client = new Utils.Network.UdpClient(listenPort);
        }

        public void Send(Google.Protobuf.IMessage message) {
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
                    byte[] messageBytes = _client.Recieve();
                    Any message = MessagesHelpers.ConvertBytesToMessage(messageBytes);
                    Debug.Log("Got message, inserting to queue");
                    MessagesQueue.Enqueue(message);
                }
            }
        }

        public void Dispose() {
            Stop();
            _client.Dispose();
        }
    }
}