using System.Collections.Concurrent;
using System.Collections.Generic;
using Assets.Scripts.Network.Common;
using Assets.Scripts.Utils;
using Assets.Scripts.Utils.Messages;
using Google.Protobuf;
using UnityEngine;

namespace Assets.Scripts.Network.Client {
    public class NetworkClientCommunicator : IClientCommunicator {
        private readonly NetworkClientCommunicatorReceiverThread _networkClientCommunicatorReceiverThread;
        private readonly NetworkClientCommunicatorSenderThread _networkClientCommunicatorSenderThread;
        private readonly ConcurrentQueue<MessageToReceive> _receiveMessagesQueue;
        private readonly ConcurrentQueue<IMessage> _sendMessagesQueue;

        public NetworkClientCommunicator(IMessageBasedClient messageBasedNetworkClient) {
            _receiveMessagesQueue = new ConcurrentQueue<MessageToReceive>();
            _sendMessagesQueue = new ConcurrentQueue<IMessage>();
            NetworkClient = messageBasedNetworkClient;
            _networkClientCommunicatorSenderThread = new NetworkClientCommunicatorSenderThread(this);
            _networkClientCommunicatorSenderThread.Start();
            _networkClientCommunicatorReceiverThread = new NetworkClientCommunicatorReceiverThread(this);
            _networkClientCommunicatorReceiverThread.Start();
        }

        public IMessageBasedClient NetworkClient { get; }

        public void Dispose() {
            _networkClientCommunicatorReceiverThread?.Stop();
            _networkClientCommunicatorSenderThread?.Stop();
            NetworkClient?.Dispose();
        }

        public MessageToReceive Receive() {
            return EnumerableUtils.TryDequeue(_receiveMessagesQueue);
        }

        public List<MessageToReceive> ReceiveAll() {
            return EnumerableUtils.DequeueAllQueue(_receiveMessagesQueue);
        }

        public void Send(IMessage message) {
            Debug.Log("Network client enqueue messageToReceive to send");
            _sendMessagesQueue.Enqueue(message);
        }

        public void AddMessageToReceive(MessageToReceive messageToReceive) {
            _receiveMessagesQueue.Enqueue(messageToReceive);
        }

        public IMessage GetMessageToSend() {
            return EnumerableUtils.TryDequeue(_sendMessagesQueue);
        }
    }
}