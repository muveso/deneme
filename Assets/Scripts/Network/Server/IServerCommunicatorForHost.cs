using Assets.Scripts.Utils.Messages;

namespace Assets.Scripts.Network.Server {
    public interface IServerCommunicatorForHost {
        void HostConnect(string nickname);
        public void AddMessageToReceive(MessageToReceive messageToReceive);
    }
}