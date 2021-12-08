using System;
using Assets.Scripts.Utils.Messages;
using Google.Protobuf;

namespace Assets.Scripts.Network.Common {
    public interface IClientCommunicator : IDisposable {
        MessageToReceive Receive();
        void Send(IMessage message);
    }
}