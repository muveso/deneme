using Assets.Scripts.Utils.Messages;

namespace Assets.Scripts.Network.Common {
    public interface IInternalMessageWriter {
        public void AddMessageToReceive(MessageToReceive messageToReceive);
    }
}