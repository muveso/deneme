[System.Serializable]
public class SocketNotConnectedException : System.Exception
{
    public SocketNotConnectedException() { }
    public SocketNotConnectedException(string message) : base(message) { }
    public SocketNotConnectedException(string message, System.Exception inner) : base(message, inner) { }
    protected SocketNotConnectedException(
        System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}