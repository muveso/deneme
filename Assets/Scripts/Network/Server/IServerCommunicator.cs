using Assets.Scripts.Network.Common;

namespace Assets.Scripts.Network.Server {
    public interface IServerCommunicatorForHost : IInternalMessageWriter {
        void HostConnect(string nickname);
    }
}