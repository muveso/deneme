using System;

namespace Assets.Scripts.Network.Common {
    public interface IClientCommunicator : IMessageReader, IMessageWriter, IDisposable { }
}