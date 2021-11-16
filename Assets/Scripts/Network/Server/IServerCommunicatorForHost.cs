using Assets.Scripts.Utils;

namespace Assets.Scripts.Network.Server {
    public interface IServerCommunicatorForHost {
        void InsertToQueue(Message message);
        void HostConnect(string nickname);
    }
}