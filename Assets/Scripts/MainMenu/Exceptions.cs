using System;
using System.Runtime.Serialization;

namespace Assets.Scripts.MainMenu {
    [Serializable]
    public class ClientNotConnectedException : Exception {
        public ClientNotConnectedException() { }
        public ClientNotConnectedException(string message) : base(message) { }
        public ClientNotConnectedException(string message, Exception inner) : base(message, inner) { }

        protected ClientNotConnectedException(
            SerializationInfo info,
            StreamingContext context) : base(info, context) { }
    }
}