using System;
using System.Collections.Generic;
using Assets.Scripts.Network.Common;
using Assets.Scripts.Network.Server;
using Assets.Scripts.Utils.Messages;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using UnityEngine;

namespace Assets.Scripts.Network.Host {
    public class HostClientCommunicator : IClientCommunicator {
        private readonly IServerCommunicatorForHost _serverCommunicator;

        public HostClientCommunicator(IServerCommunicatorForHost serverCommunicator, string nickname) {
            _serverCommunicator = serverCommunicator;
            _serverCommunicator.HostConnect(nickname);
        }

        public void Send(IMessage message) {
            var messageToInsert = new MessageToReceive(HostClient.GetHostClientEndpoint(), Any.Pack(message));
            Debug.Log("Host client inserting message to queue");
            _serverCommunicator.AddMessageToReceive(messageToInsert);
        }

        public MessageToReceive Receive() {
            throw new NotImplementedException();
        }

        public List<MessageToReceive> ReceiveAll() {
            throw new NotImplementedException();
        }

        public void Dispose() { }
    }
}