using System.Net;
using Google.Protobuf.WellKnownTypes;

namespace Assets.Scripts.Utils.Messages {
    public class MessageToReceive {
        public MessageToReceive(IPEndPoint ipEndpoint, Any anyMessage) {
            IPEndpoint = ipEndpoint;
            AnyMessage = anyMessage;
        }

        public IPEndPoint IPEndpoint { get; }
        public Any AnyMessage { get; }
    }
}