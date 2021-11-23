using System;
using System.Net.Sockets;
using Assets.Scripts.Utils;
using Assets.Scripts.Utils.Messages;
using Assets.Scripts.Utils.Network;
using Google.Protobuf.WellKnownTypes;
using UnityEngine;

namespace Assets.Scripts.Network.Client {
    public class NetworkClientCommunicatorReceiverThread : BaseThread {
        private readonly NetworkClientCommunicator _networkClientCommunicator;

        public NetworkClientCommunicatorReceiverThread(NetworkClientCommunicator networkClientCommunicator) {
            _networkClientCommunicator = networkClientCommunicator;
        }

        protected override void RunThread() {
            while (ThreadShouldRun) {
                try {
                    var message = _networkClientCommunicator.NetworkClient.Receive(false);
                    if (message != null) {
                        _networkClientCommunicator.AddMessageToReceive(message);
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
            _networkClientCommunicator.AddMessageToReceive(disconnectedMessage);
            Stop(false);
        }
    }
}