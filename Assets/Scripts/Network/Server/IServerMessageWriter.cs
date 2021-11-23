using System.Collections.Generic;
using System.Net;
using Google.Protobuf;

namespace Assets.Scripts.Network.Server {
    public interface IServerMessageWriter {
        void Send(IMessage message, List<IPEndPoint> clients);
        void SendAll(IMessage message);
    }
}