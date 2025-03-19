using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using DS.Core.Enums;
using DS.Core.Interfaces;
using DS.Models;
using UnityEngine;

namespace DS.Core.Sync
{
    public class SyncManager : IDisposable
    {
        private readonly CancellationTokenSource _cts = new();
        private readonly ConcurrentDictionary<(SyncTarget Target, string Key), SyncJob> _pendingJobs = new();
        private readonly ISyncStrategy[] _strategies;

        public SyncManager(params ISyncStrategy[] strategies)
        {
            _strategies = strategies;
        }

        public void Dispose()
        {
            _cts.Cancel();
            _cts.Dispose();
            _pendingJobs.Clear();
            Debug.Log("SyncManager disposed");
        }

        public void AddJobInQueue(SyncTarget target, SyncJob job)
        {
            // Сохраняем только последнюю задачу для каждого ключа
            _pendingJobs.AddOrUpdate(
                (target, job.Key),
                job,
                (key, oldJob) => job
            );
        }

        public void AddJobsInQueue(SyncTarget target, string[] keys, DataEntity[] data)
        {
            for (var i = 0; i < keys.Length; i++)
            {
                var j = new SyncJob(keys[i], data[i]);
                AddJobInQueue(target, j);
            }
        }

        internal async UniTask<Result> ProcessQueueAsync(SyncTarget target, CancellationToken token = default)
        {
            var strategy = _strategies.FirstOrDefault(s => s.Handles(target));
            if (strategy == null) return Result.Failure("Sync strategy not found");

            try
            {
                var jobsToProcess = _pendingJobs
                    .Where(kvp => kvp.Key.Target == target)
                    .ToList();

                foreach (var (key, _) in jobsToProcess) _pendingJobs.TryRemove(key, out _);

                foreach (var (_, job) in jobsToProcess)
                {
                    var result = await strategy.SyncIt(job);

                    if (!result.IsSuccess)
                        return Result.Failure($"Sync failed for target {target}: {result.ErrorMessage}");
                }
            }
            catch (OperationCanceledException)
            {
                return Result.Failure($"Processing for target {target} canceled.");
            }
            catch (Exception ex)
            {
                return Result.Failure($"Unexpected error in sync process: {ex.Message}");
            }

            return Result.Success();
        }
    }
}