using System;
using Assets.Scripts.Network.Common;
using Assets.Scripts.Network.Server;
using Assets.Scripts.Utils;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;

namespace Assets.Scripts.Network.Host {
    public class HostClientCommunicator : IClientCommunicator {
        private readonly IServerCommunicatorForHost _serverCommunicator;

        public HostClientCommunicator(IServerCommunicatorForHost serverCommunicator, string nickname) {
            _serverCommunicator = serverCommunicator;
            _serverCommunicator.HostConnect(nickname);
        }

        public void Send(IMessage message) {
            var messageToInsert = new Message(HostClient.GetHostClientEndpoint(), Any.Pack(message));
            _serverCommunicator.InsertToQueue(messageToInsert);
        }

        public Message GetMessage() {
            throw new NotImplementedException();
        }

        public void Dispose() {
            throw new NotImplementedException();
        }
    }
}