using System.Net;

namespace Evade {
    public class GameGlobals {
        public static bool IsHost;

        public static void Reset() {
            IsHost = false;
        }
    }

    public class ClientGlobals {
        public static IPEndPoint ServerEndpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 5555);
    }
}