using System;
using System.Collections.Concurrent;

namespace _Project.System.DataManagementService
{
    public class SyncManager {
        private readonly ConcurrentDictionary<SyncTarget, ConcurrentQueue<SyncJob>> _queues = new();
        private readonly ISyncStrategy[] _strategies;

        public SyncManager(params ISyncStrategy[] strategies) {
            _strategies = strategies;
            foreach (SyncTarget target in Enum.GetValues(typeof(SyncTarget))) {
                _queues[target] = new ConcurrentQueue<SyncJob>();
            }
        }

        public void Queue(SyncTarget target, SyncJob job) {
            _queues[target].Enqueue(job);
        }

        public void ProcessQueue(SyncTarget target) {
            if (_queues.TryGetValue(target, out var queue)) {
                while (queue.TryDequeue(out var job)) {
                    var strategy = Array.Find(_strategies, s => s.Handles(target));
                    try {
                        strategy.Execute(job);
                        job.OnComplete?.Invoke();
                    } catch (Exception ex) {
                        job.OnError?.Invoke(ex);
                    }
                }
            }
        }
    }
}