
using System.Collections.Concurrent;

namespace AIInfluence.Services
{
    public static class AIDebugService
    {
        private static readonly ConcurrentQueue<string> _queue = new();

        public static void Log(string message) => _queue.Enqueue(message);

        public static void Flush(System.Action<string> sink)
        {
            while (_queue.TryDequeue(out var msg))
                sink(msg);
        }
    }
}
