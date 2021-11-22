using Assets.Scripts.Network.Common;

namespace Assets.Scripts.Network.Server {
    public interface IServerCommunicator : IMessageWriter {
        void HostConnect(string nickname);
    }
}