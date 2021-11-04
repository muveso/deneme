using Google.Protobuf;

namespace Utils {
    public static class MessagesHelpers {
        public static byte[] WrapMessage(Google.Protobuf.IMessage message) {
            BaseMessage baseMessage = new BaseMessage();
            baseMessage.Message = Google.Protobuf.WellKnownTypes.Any.Pack(message);
            return baseMessage.ToByteArray();
        }
    }
}
