using System;
using System.Collections.Generic;
using Assets.Scripts.Utils.Messages;
using Google.Protobuf;

namespace Assets.Scripts.Network.Common {
    public interface IClientManager : IDisposable {
        List<MessageToReceive> ReceiveAll();
        MessageToReceive Receive();
        void Send(IMessage message);
    }
}