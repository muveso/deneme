using System.Collections.Generic;
using Assets.Scripts.Utils;

namespace Assets.Scripts.Network.Common {
    public interface IMessageReader {
        Message GetMessage();
        List<Message> GetAllMessages();
    }
}