using System;
using System.Runtime.Serialization;

namespace Assets.Scripts.Utils.Network {
    [Serializable]
    public class SocketNotConnectedException : Exception {
        public SocketNotConnectedException() { }
        public SocketNotConnectedException(string message) : base(message) { }
        public SocketNotConnectedException(string message, Exception inner) : base(message, inner) { }

        protected SocketNotConnectedException(
            SerializationInfo info,
            StreamingContext context) : base(info, context) { }
    }

    [Serializable]
    public class SocketClosedException : Exception {
        public SocketClosedException() { }
        public SocketClosedException(string message) : base(message) { }
        public SocketClosedException(string message, Exception inner) : base(message, inner) { }

        protected SocketClosedException(
            SerializationInfo info,
            StreamingContext context) : base(info, context) { }
    }
}