using System;
using Assets.Scripts.Utils.Messages;
using Google.Protobuf;

namespace Assets.Scripts.Network.Client {
    public interface IMessageBasedClient : IDisposable {
        public void Send(IMessage message);
        public MessageToReceive Receive(bool block = true);
    }
}