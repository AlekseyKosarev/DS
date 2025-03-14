using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using _Project.System.DS.Core.Enums;
using _Project.System.DS.Core.Interfaces;
using _Project.System.DS.Models;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.System.DS.Core.Sync
{
    public class SyncManager : IDisposable
    {
        private readonly ConcurrentDictionary<(SyncTarget Target, string Key), SyncJob> _pendingJobs = new();
        private readonly ISyncStrategy[] _strategies;
        private readonly CancellationTokenSource _cts = new();

        public SyncManager(params ISyncStrategy[] strategies)
        {
            _strategies = strategies;
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
        internal async UniTask<Result> ProcessQueueAsync(SyncTarget target, CancellationToken token = default)
        {
            var strategy = _strategies.FirstOrDefault(s => s.Handles(target));
            if (strategy == null) return Result.Failure("Sync strategy not found");

            try
            {
                var jobsToProcess = _pendingJobs
                    .Where(kvp => kvp.Key.Target == target)
                    .ToList();

                foreach (var (key, _) in jobsToProcess)
                {
                    _pendingJobs.TryRemove(key, out _);
                }

                foreach (var (_, job) in jobsToProcess)
                {
                    var result = await strategy.SyncIt(job);

                    if (!result.IsSuccess)
                    {
                        return Result.Failure($"Sync failed for target {target}: {result.ErrorMessage}");
                    }
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
        public void Dispose()
        {
            _cts.Cancel();
            _cts.Dispose();
            _pendingJobs.Clear();
            Debug.Log("SyncManager disposed");
        }
    }
}