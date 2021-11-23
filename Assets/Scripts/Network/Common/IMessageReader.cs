using System.Collections.Generic;
using Assets.Scripts.Utils.Messages;

namespace Assets.Scripts.Network.Common {
    public interface IMessageReader {
        MessageToReceive Receive();
        List<MessageToReceive> ReceiveAll();
    }
}