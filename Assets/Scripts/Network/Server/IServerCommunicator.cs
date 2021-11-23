using Assets.Scripts.Network.Common;

namespace Assets.Scripts.Network.Server {
    public interface IServerCommunicatorForHost : IInternalMessageWrite {
        void HostConnect(string nickname);
    }
}