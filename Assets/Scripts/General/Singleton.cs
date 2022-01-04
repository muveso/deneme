namespace Assets.Scripts.General {
    public class Singleton<T> where T : new() {
        private static readonly object _padlock = new();
        private static T _instance;

        public static T Instance {
            get {
                // There is no reason to lock if the instance is already initialized
                if (_instance == null) {
                    // Lock to make sure that only one thread will create instance
                    lock (_padlock) {
                        if (_instance == null) {
                            _instance = new T();
                        }
                    }
                }

                return _instance;
            }
        }
    }
}