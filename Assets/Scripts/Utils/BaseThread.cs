using System.Threading;
using UnityEngine;

namespace Assets.Scripts.Utils {
    public abstract class BaseThread {
        private const int TimeoutToWaitBeforeForceTerminateThreadMs = 5000;
        private readonly AutoResetEvent _stopEvent;
        private readonly Thread _thread;

        protected BaseThread() {
            _thread = new Thread(RunThread);
            _stopEvent = new AutoResetEvent(false);
        }

        public bool IsAlive => _thread.IsAlive;

        protected bool ThreadShouldRun => !_stopEvent.WaitOne(0);

        public void Start() {
            _thread.Start();
        }

        public void Stop(bool wait = true) {
            _stopEvent.Set();
            if (!wait) {
                return;
            }

            if (!_thread.Join(TimeoutToWaitBeforeForceTerminateThreadMs)) {
                Debug.LogError("Thread got timeout to stop so terminating :(");
                _thread.Abort();
            } else {
                Debug.Log("Thread stopped successfully :)");
            }
        }

        protected abstract void RunThread();
    }
}