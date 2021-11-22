using System;
using Assets.Scripts.Utils;
using Google.Protobuf;

namespace Assets.Scripts.Network.Client {
    public interface IMessageBasedClient : IDisposable {
        public void Send(IMessage message);
        public Message Receive(bool block = true);
    }
}