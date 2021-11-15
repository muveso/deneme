using System.Net;

namespace Evade {
    public class GameGlobals {
        public static bool IsHost;

        public static void Reset() {
            IsHost = false;
        }
    }

    public class ClientGlobals {
        public static string Nickname = "DefaultNickname";
        public static IPEndPoint ServerEndpoint;
    }
}