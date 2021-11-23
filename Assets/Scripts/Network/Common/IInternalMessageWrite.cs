using Assets.Scripts.Utils.Messages;

namespace Assets.Scripts.Network.Common {
    public interface IInternalMessageWrite {
        public void AddMessageToReceive(MessageToReceive messageToReceive);
    }
}