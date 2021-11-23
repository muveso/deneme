using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Assets.Scripts.Utils {
    public static class EnumerableUtils {
        public static List<T> DequeueAllQueue<T>(ConcurrentQueue<T> concurrentQueue) {
            var messages = new List<T>();
            while (!concurrentQueue.IsEmpty) {
                if (concurrentQueue.TryDequeue(out var message)) {
                    messages.Add(message);
                }
            }

            return messages;
        }
    }
}