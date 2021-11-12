namespace Evade {
    public class ClientDetails {
        public ClientDetails() { }

        public ClientDetails(string nickname, bool isReady) {
            Nickname = nickname;
            IsReady = isReady;
        }

        public string Nickname { get; set; }
        public bool IsReady { get; set; }

        public void ToggleReady() {
            IsReady = !IsReady;
        }

        public override string ToString() {
            return $"Nickname: {Nickname} | Ready: {IsReady}";
        }
    }
}