using System;
using Google.Protobuf;

namespace Assets.Scripts.Network.Common {
    public interface IClientCommunicator : IMessageReader, IDisposable {
        void Send(IMessage message);
    }
}