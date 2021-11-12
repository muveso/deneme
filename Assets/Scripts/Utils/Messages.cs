using Google.Protobuf;
using Google.Protobuf.Reflection;
using Google.Protobuf.WellKnownTypes;

namespace Evade.Utils {
    public static class MessagesHelpers {
        public static byte[] ConvertMessageToBytes(IMessage message) {
            var baseMessage = new BaseMessage();
            baseMessage.Message = Any.Pack(message);
            return baseMessage.ToByteArray();
        }

        public static Any ConvertBytesToMessage(byte[] message) {
            var baseMessage = BaseMessage.Parser.ParseFrom(message);
            return baseMessage.Message;
        }

        public static bool IsMessageTypeOf(BaseMessage baseMessage, MessageDescriptor descriptorToCheck) {
            return baseMessage.Message.Is(descriptorToCheck);
        }
    }
}