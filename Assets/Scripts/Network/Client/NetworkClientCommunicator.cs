using System;
using System.Collections.Concurrent;
using Assets.Scripts.Network.Common;
using Assets.Scripts.Utils;
using Assets.Scripts.Utils.Messages;
using Google.Protobuf;
using UnityEngine;

namespace Assets.Scripts.Network.Client {
    public class NetworkClientCommunicator : ICommunicator {
        private readonly NetworkClientCommunicatorReceiverThread _networkClientCommunicatorReceiverThread;
        private readonly NetworkClientCommunicatorSenderThread _networkClientCommunicatorSenderThread;

        public NetworkClientCommunicator(IMessageBasedClient messageBasedNetworkClient) {
            ReceiveMessagesQueue = new ConcurrentQueue<MessageToReceive>();
            SendMessagesQueue = new ConcurrentQueue<IMessage>();
            NetworkClient = messageBasedNetworkClient;
            _networkClientCommunicatorSenderThread = new NetworkClientCommunicatorSenderThread(this);
            _networkClientCommunicatorSenderThread.Start();
            _networkClientCommunicatorReceiverThread = new NetworkClientCommunicatorReceiverThread(this);
            _networkClientCommunicatorReceiverThread.Start();
        }

        public ConcurrentQueue<IMessage> SendMessagesQueue { get; }

        public ConcurrentQueue<MessageToReceive> ReceiveMessagesQueue { get; }

        public IMessageBasedClient NetworkClient { get; }

        public void Dispose() {
            _networkClientCommunicatorReceiverThread?.Stop();
            _networkClientCommunicatorSenderThread?.Stop();
            NetworkClient?.Dispose();
        }

        public MessageToReceive Receive() {
            return EnumerableUtils.TryDequeue(ReceiveMessagesQueue);
        }

        public void Send(IMessage message) {
            Debug.Log("Network client enqueue messageToReceive to send");
            SendMessagesQueue.Enqueue(message);
        }

        public void Send(MessageToSend message) {
            throw new NotImplementedException();
        }
    }
}