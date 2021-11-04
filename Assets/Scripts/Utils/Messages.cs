using Google.Protobuf;
using Google.Protobuf.Reflection;

namespace Utils {
    public static class MessagesHelpers {
        public static byte[] WrapMessage(Google.Protobuf.IMessage message) {
            BaseMessage baseMessage = new BaseMessage();
            baseMessage.Message = Google.Protobuf.WellKnownTypes.Any.Pack(message);
            return baseMessage.ToByteArray();
        }

        public static BaseMessage GetBaseMessage(byte[] message) {
            BaseMessage baseMessage = BaseMessage.Parser.ParseFrom(message);
            return baseMessage;
        }

        public static bool IsMessageTypeOf(BaseMessage baseMessage, MessageDescriptor descriptorToCheck) {
            return baseMessage.Message.Is(descriptorToCheck);
        }
    }
}
