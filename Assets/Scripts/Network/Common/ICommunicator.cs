using Assets.Scripts.Utils.Messages;
using Google.Protobuf;

namespace Assets.Scripts.Network.Common {
    public interface ICommunicator {
        MessageToReceive Receive();
        void Send(IMessage message);
        void Send(MessageToSend message);
    }
}