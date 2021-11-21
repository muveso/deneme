using System;
using System.Collections.Generic;
using Assets.Scripts.Utils;
using Google.Protobuf;

namespace Assets.Scripts.Network.Common {
    public interface IClientCommunicator : IDisposable {
        void Send(IMessage message);

        Message Receive();
        List<Message> ReceiveAll();
    }
}