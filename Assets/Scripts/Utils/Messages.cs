using Google.Protobuf;
using Google.Protobuf.Reflection;
using Google.Protobuf.WellKnownTypes;

namespace Utils {
    public static class MessagesHelpers {
        public static byte[] ConvertMessageToBytes(Google.Protobuf.IMessage message) {
            BaseMessage baseMessage = new BaseMessage();
            baseMessage.Message = Google.Protobuf.WellKnownTypes.Any.Pack(message);
            return baseMessage.ToByteArray();
        }

        public static Any ConvertBytesToMessage(byte[] message) {
            BaseMessage baseMessage = BaseMessage.Parser.ParseFrom(message);
            return baseMessage.Message;
        }

        public static bool IsMessageTypeOf(BaseMessage baseMessage, MessageDescriptor descriptorToCheck) {
            return baseMessage.Message.Is(descriptorToCheck);
        }
    }
}
