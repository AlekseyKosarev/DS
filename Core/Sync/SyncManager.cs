using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using DS.Core.Enums;
using DS.Core.Interfaces;

namespace DS.Core.Sync
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
        public async UniTask ProcessQueueAsync(SyncTarget target, CancellationToken token) {
            if (_queues.TryGetValue(target, out var queue)) {
                while (queue.TryDequeue(out var job)) {
                    token.ThrowIfCancellationRequested();
                    var strategy = _strategies.First(s => s.Handles(target));
                    await strategy.ExecuteAsync(job, token);
                }
            }
        }

        // public void ProcessQueue(SyncTarget target) {
        //     if (_queues.TryGetValue(target, out var queue)) {
        //         while (queue.TryDequeue(out var job)) {
        //             var strategy = Array.Find(_strategies, s => s.Handles(target));
        //             try {
        //                 strategy.ExecuteAsync(job);
        //                 job.OnComplete?.Invoke();
        //             } catch (Exception ex) {
        //                 job.OnError?.Invoke(ex);
        //             }
        //         }
        //     }
        // }
    }
}