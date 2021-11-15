using System.Net;

namespace Evade {
    public class GameConsts {
        public const string DefaultServerIpAddress = "0.0.0.0";
        public const string LocalHostIpAddress = "127.0.0.1";
    }

    public static class GameGlobals {
        public static bool IsHost;

        public static void Reset() {
            IsHost = false;
        }
    }

    public static class ClientGlobals {
        public static string Nickname = "DefaultNickname";
        public static IPEndPoint ServerEndpoint;
    }
}