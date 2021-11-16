namespace Assets.Scripts.Utils.Network.TCP {
    public interface INetworkClient {
        void Send(byte[] bytes);
        byte[] Receive();
    }
}