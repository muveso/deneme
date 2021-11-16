using System.Net;

namespace Assets.Scripts.General {
    public class GameConsts {
        public const string DefaultServerIpAddress = "0.0.0.0";
        public const string LocalHostIpAddress = "127.0.0.1";
    }

    public static class ClientGlobals {
        public static string Nickname = "DefaultNickname";
        public static IPEndPoint ServerEndpoint;
    }
}