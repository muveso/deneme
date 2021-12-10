using System;
using System.Net.Sockets;
using Assets.Scripts.Utils;
using Assets.Scripts.Utils.Messages;
using Assets.Scripts.Utils.Network;
using Google.Protobuf.WellKnownTypes;
using UnityEngine;

namespace Assets.Scripts.Network.Client {
    public class NetworkClientCommunicatorReceiverThread : BaseThread {
        private readonly NetworkClientClientManager _networkClientClientManager;

        public NetworkClientCommunicatorReceiverThread(NetworkClientClientManager networkClientClientManager) {
            _networkClientClientManager = networkClientClientManager;
        }

        protected override void RunThread() {
            while (ThreadShouldRun) {
                try {
                    var message = _networkClientClientManager.NetworkClient.Receive(false);
                    if (message != null) {
                        _networkClientClientManager.Communicator.AddMessageToReceive(message);
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
            _networkClientClientManager.Communicator.AddMessageToReceive(disconnectedMessage);
            Stop(false);
        }
    }
}