using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using DS.Core.Enums;
using DS.Core.Interfaces;
using UnityEngine;

namespace DS.Core.Sync
{
    public class SyncManager {
        private readonly ConcurrentDictionary<SyncTarget, AsyncQueue<SyncJob>> _queues = new();
        private readonly ISyncStrategy[] _strategies;

        public SyncManager(params ISyncStrategy[] strategies) {
            _strategies = strategies;
            foreach (SyncTarget target in Enum.GetValues(typeof(SyncTarget))) {
                _queues[target] = new AsyncQueue<SyncJob>();
            }
            StartProcessing().Forget();
        }

        public void Queue(SyncTarget target, SyncJob job) {
            _queues[target].Enqueue(job);
        }

        private async UniTaskVoid StartProcessing() {
            var processes = new UniTask[] {
                ProcessQueueAsync(SyncTarget.Local),
                ProcessQueueAsync(SyncTarget.Remote)
            };
            await UniTask.WhenAll(processes);
        }

        internal async UniTask ProcessQueueAsync(SyncTarget target, CancellationToken token = default) {
            var strategy = _strategies.FirstOrDefault(s => s.Handles(target));
            if (strategy == null) return;

            while (!token.IsCancellationRequested) {
                try {
                    var job = await _queues[target].DequeueAsync(token);
                    var result = await strategy.ExecuteAsync(job);

                    if (!result.IsSuccess) 
                    {
                        Debug.LogError($"Sync failed for target {target}: {result.ErrorMessage}");
                    } 
                    else
                    {
                        Debug.Log($"Sync succeeded for target {target}.");
                    }
                } 
                catch (Exception ex)
                {
                    Debug.LogError($"Unexpected error in sync process: {ex.Message}");
                }
            }
        }
    }
}