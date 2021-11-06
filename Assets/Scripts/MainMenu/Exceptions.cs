[System.Serializable]
public class ClientNotConnectedException : System.Exception {
    public ClientNotConnectedException() { }
    public ClientNotConnectedException(string message) : base(message) { }
    public ClientNotConnectedException(string message, System.Exception inner) : base(message, inner) { }
    protected ClientNotConnectedException(
        System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}