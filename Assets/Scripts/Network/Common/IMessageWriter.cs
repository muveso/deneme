using Google.Protobuf;

namespace Assets.Scripts.Network.Common {
    public interface IMessageWriter {
        void Send(IMessage message);
    }
}