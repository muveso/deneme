using Assets.Scripts.General;
using Assets.Scripts.Network.Client;

namespace Assets.Scripts.Network.Common {
    public static class ClientMessages {
        public static void SendClientDetails(IClientCommunicator communicator) {
            var clientDetailsMessage = new ClientDetailsMessage {
                Nickname = ClientGlobals.Nickname
            };
            communicator.Send(clientDetailsMessage);
        }

        public static void SendClientReady(IClientCommunicator communicator) {
            communicator.Send(new ClientReadyMessage());
        }
    }
}