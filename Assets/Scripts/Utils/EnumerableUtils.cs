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

        public static T TryDequeue<T>(ConcurrentQueue<T> concurrentQueue) {
            concurrentQueue.TryDequeue(out var message);
            return message;
        }

        public static List<T> DequeueAllQueue<T>(Queue<T> queue) {
            var messages = new List<T>();
            while (queue.Count > 0) {
                messages.Add(TryDequeue(queue));
            }

            return messages;
        }

        public static T TryDequeue<T>(Queue<T> queue) {
            return queue.Dequeue();
        }
    }
}