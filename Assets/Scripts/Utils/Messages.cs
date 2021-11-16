using System.Net;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using UnityEngine;

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
        public static Vector3Message CreateVector3Message(Vector3 vector) {
            var message = new Vector3Message {
                X = vector.x,
                Y = vector.y,
                Z = vector.z
            };
            return message;
        }

        public static Vector3 CreateVector3FromMessage(Vector3Message vector) {
            return new Vector3(vector.X, vector.Y, vector.Z);
        }

        public static byte[] ConvertMessageToBytes(IMessage message) {
            return Any.Pack(message).ToByteArray();
        }

        public static Any ConvertBytesToMessage(byte[] message) {
            var baseMessage = Any.Parser.ParseFrom(message);
            return baseMessage;
        }
    }
}