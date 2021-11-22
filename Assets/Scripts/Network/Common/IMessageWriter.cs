using Assets.Scripts.Utils;

namespace Assets.Scripts.Network.Common {
    public interface IMessageWriter {
        void AddMessage(Message message);
    }
}