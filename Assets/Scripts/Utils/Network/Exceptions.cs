namespace Utils.Network {
    [System.Serializable]
    public class SocketNotConnectedException : System.Exception {
        public SocketNotConnectedException() { }
        public SocketNotConnectedException(string message) : base(message) { }
        public SocketNotConnectedException(string message, System.Exception inner) : base(message, inner) { }
        protected SocketNotConnectedException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

    [System.Serializable]
    public class SocketClosedException : System.Exception {
        public SocketClosedException() { }
        public SocketClosedException(string message) : base(message) { }
        public SocketClosedException(string message, System.Exception inner) : base(message, inner) { }
        protected SocketClosedException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
