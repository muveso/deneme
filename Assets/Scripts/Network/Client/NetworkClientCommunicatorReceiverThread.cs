using System;
using System.Net.Sockets;
using Assets.Scripts.Utils;
using Assets.Scripts.Utils.Messages;
using Assets.Scripts.Utils.Network;
using Google.Protobuf.WellKnownTypes;
using UnityEngine;

namespace Assets.Scripts.Network.Client {
    public class NetworkClientCommunicatorReceiverThread : BaseThread {
        private readonly NetworkClientManager _networkClientManager;

        public NetworkClientCommunicatorReceiverThread(NetworkClientManager networkClientManager) {
            _networkClientManager = networkClientManager;
        }

        protected override void RunThread() {
            while (ThreadShouldRun) {
                try {
                    var message = _networkClientManager.NetworkClient.Receive(1 * 1000, false);
                    if (message != null) {
                        _networkClientManager.Communicator.AddMessageToReceive(message);
                    }
                } catch (Exception exception) {
                    if (exception is SocketException || exception is SocketClosedException) {
                        HandleServerDisconnected();
                    } else {
                        throw;
                    }
                }
            }
        }

        private void HandleServerDisconnected() {
            Debug.Log("Exception: Server disconnected! :(");
            var disconnectedMessage = new MessageToReceive(null, Any.Pack(new ServerDisconnectMessage()));
            _networkClientManager.Communicator.AddMessageToReceive(disconnectedMessage);
            Stop(false);
        }
    }
}