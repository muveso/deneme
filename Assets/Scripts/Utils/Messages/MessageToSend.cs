using System.Collections.Generic;
using System.Net;
using Google.Protobuf;

namespace Assets.Scripts.Utils.Messages {
    public class MessageToSend {
        public MessageToSend(List<IPEndPoint> ipEndpoints, IMessage message) {
            IPEndpoints = ipEndpoints;
            Message = message;
        }

        public List<IPEndPoint> IPEndpoints { get; }
        public IMessage Message { get; }
    }
}