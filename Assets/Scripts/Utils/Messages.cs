using System.Net;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;

namespace Assets.Scripts.Utils {
    public class Message {
        public Message(IPEndPoint ipEndpoint, Any protobuffMessage) {
            IPEndpoint = ipEndpoint;
            ProtobufMessage = protobuffMessage;
        }

        public IPEndPoint IPEndpoint { get; }
        public Any ProtobufMessage { get; }
    }

    public static class MessagesHelpers {
        public static byte[] ConvertMessageToBytes(IMessage message) {
            return Any.Pack(message).ToByteArray();
        }

        public static Any ConvertBytesToMessage(byte[] message) {
            var baseMessage = Any.Parser.ParseFrom(message);
            return baseMessage;
        }
    }
}