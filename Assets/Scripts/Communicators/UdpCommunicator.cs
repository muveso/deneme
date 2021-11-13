using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using Evade.Utils;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using UnityEngine;
using UdpClient = Evade.Utils.Network.UdpClient;

namespace Evade.Communicators {
    public class UdpCommunicator : BaseThread, IDisposable {
        private const int PollTimeoutMs = 1000;
        protected UdpClient Client;

        public UdpCommunicator(string ipAddress, int port) {
            MessagesQueue = new ConcurrentQueue<Any>();
            Client = new UdpClient(ipAddress, port);
        }

        public UdpCommunicator(int listenPort) {
            MessagesQueue = new ConcurrentQueue<Any>();
            Client = new UdpClient(listenPort);
        }

        public ConcurrentQueue<Any> MessagesQueue { get; }

        public void Dispose() {
            Stop();
            Client.Dispose();
        }

        public void Send(IMessage message) {
            Client.Send(MessagesHelpers.ConvertMessageToBytes(message));
        }

        public Any TryGetMessageFromQueue() {
            Any message;
            MessagesQueue.TryDequeue(out message);
            return message;
        }

        protected override void RunThread() {
            while (ThreadShouldRun) {
                if (Client.Sock.Poll(PollTimeoutMs, SelectMode.SelectRead)) {
                    var endPoint = new IPEndPoint(IPAddress.Any, 0);
                    var messageBytes = Client.Receive(ref endPoint);
                    PreHandleMessage(endPoint, messageBytes);
                    var message = MessagesHelpers.ConvertBytesToMessage(messageBytes);
                    Debug.Log("Got message, inserting to queue");
                    MessagesQueue.Enqueue(message);
                }
            }
        }

        protected virtual void PreHandleMessage(IPEndPoint endPoint, byte[] messageBytes) { }
    }
}