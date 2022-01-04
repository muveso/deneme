using System;
using System.Collections.Generic;
using Assets.Scripts.Utils.Messages;
using Google.Protobuf;

namespace Assets.Scripts.Network.Common {
    public interface IClientManager : IDisposable {
        MessageToReceive Receive();
        List<MessageToReceive> ReceiveAll();
        void Send(IMessage message);
    }
}