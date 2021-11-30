namespace Assets.Scripts.General {
    public class GameConsts {
        public const string DefaultServerIpAddress = "0.0.0.0";
        public const int DefaultUdpServerPort = 55556;
        public const float TickRate = 1f / 60f;
    }

    public static class ClientGlobals {
        public static string Nickname = "DefaultNickname";
    }
}