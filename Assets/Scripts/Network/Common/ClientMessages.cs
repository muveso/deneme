using Assets.Scripts.General;

namespace Assets.Scripts.Network.Common {
    public static class ClientMessages {
        public static void SendClientDetails(IClientManager clientManager) {
            var clientDetailsMessage = new ClientDetailsMessage {
                Nickname = ClientGlobals.Nickname
            };
            clientManager.Send(clientDetailsMessage);
        }

        public static void SendClientReady(IClientManager clientManager) {
            clientManager.Send(new ClientReadyMessage());
        }
    }
}