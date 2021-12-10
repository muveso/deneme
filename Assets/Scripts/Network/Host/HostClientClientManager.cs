using System;
using Assets.Scripts.Network.Common;
using Assets.Scripts.Network.Server;
using Assets.Scripts.Utils.Messages;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using UnityEngine;

namespace Assets.Scripts.Network.Host {
    public class HostClientClientManager : IClientManager {
        private readonly IServerCommunicatorForHost _serverCommunicator;

        public HostClientClientManager(IServerCommunicatorForHost serverCommunicator, string nickname) {
            _serverCommunicator = serverCommunicator;
            _serverCommunicator.HostConnect(nickname);
        }

        public void Send(IMessage message) {
            var messageToInsert = new MessageToReceive(HostClient.GetHostClientEndpoint(), Any.Pack(message));
            Debug.Log("Host client inserting message to queue");
            _serverCommunicator.AddMessageToReceive(messageToInsert);
        }

        public void Send(MessageToSend message) {
            throw new NotImplementedException();
        }

        public MessageToReceive Receive() {
            throw new NotImplementedException();
        }

        public void Dispose() { }
    }
}