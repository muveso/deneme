using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Assets.Scripts.Utils {
    public static class EnumerableUtils {
        public static List<T> DequeueAllQueue<T>(BlockingCollection<T> concurrentQueue) {
            var messages = new List<T>();
            while (concurrentQueue.Count > 0) {
                if (concurrentQueue.TryTake(out var message)) {
                    messages.Add(message);
                }
            }

            return messages;
        }
        
        public static T TryDequeue<T>(BlockingCollection<T> concurrentQueue, int millisecondsTimeout = 0) {
            concurrentQueue.TryTake(out var message, millisecondsTimeout);
            return message;
        }

    }
}