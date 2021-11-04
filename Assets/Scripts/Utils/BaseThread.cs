using System.Threading;
using UnityEngine;

namespace Utils {
    public abstract class BaseThread {
        private Thread _thread;
        private AutoResetEvent _stopEvent;

        const int TIMEOUT_TO_WAIT_BEFORE_TERMINATE_THREAD_MS = 5000;

        public BaseThread() {
            _thread = new Thread(RunThread);
            _stopEvent = new AutoResetEvent(false);
        }

        public void Start() => _thread.Start();
        public void Abort() => _thread.Abort();
        public void Join() => _thread.Join();
        public bool IsAlive => _thread.IsAlive;
        public void Stop() {
            _stopEvent.Set();
            if (!_thread.Join(TIMEOUT_TO_WAIT_BEFORE_TERMINATE_THREAD_MS)) {
                Debug.Log("Thread got timeout to stop so terminating :(");
                _thread.Abort();
            } else {
                Debug.Log("Thread stopped successfully :)");
            }
        }

        protected bool ShouldRun() {
            return !_stopEvent.WaitOne(0);
        }
        protected abstract void RunThread();
    }
}
