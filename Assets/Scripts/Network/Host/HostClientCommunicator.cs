using System;
using System.Collections.Generic;
using Assets.Scripts.Network.Client;
using Assets.Scripts.Network.Common;
using Assets.Scripts.Network.Server;
using Assets.Scripts.Utils;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using UnityEngine;

namespace Assets.Scripts.Network.Host {
    public class HostClientCommunicator : IClientCommunicator {
        private readonly IServerCommunicator _serverCommunicator;

        public HostClientCommunicator(IServerCommunicator serverCommunicator, string nickname) {
            _serverCommunicator = serverCommunicator;
            _serverCommunicator.HostConnect(nickname);
        }

        public void Send(IMessage message) {
            var messageToInsert = new Message(HostClient.GetHostClientEndpoint(), Any.Pack(message));
            Debug.Log("Host client inserting message to queue");
            _serverCommunicator.AddMessage(messageToInsert);
        }

        public Message Receive(bool block = true) {
            throw new NotImplementedException();
        }

        public Message GetMessage() {
            throw new NotImplementedException();
        }

        public List<Message> GetAllMessages() {
            throw new NotImplementedException();
        }

        public void Dispose() { }
    }
}