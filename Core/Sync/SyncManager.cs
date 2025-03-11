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
    public class SyncManager : IDisposable
    {
        // Используем ConcurrentDictionary для хранения задач
        private readonly ConcurrentDictionary<(SyncTarget Target, string Key), SyncJob> _pendingJobs = new();
        private readonly ISyncStrategy[] _strategies;
        private readonly CancellationTokenSource _cts = new();

        public SyncManager(params ISyncStrategy[] strategies)
        {
            _strategies = strategies;
        }

        /// <summary>
        /// Добавление задачи в очередь
        /// </summary>
        public void Queue(SyncTarget target, SyncJob job)
        {
            // Сохраняем только последнюю задачу для каждого ключа
            _pendingJobs.AddOrUpdate(
                (target, job.Key),
                job,
                (key, oldJob) => job // Заменяем старую задачу новой
            );
        }

        /// <summary>
        /// Обработка очереди задач
        /// </summary>
        internal async UniTask ProcessQueueAsync(SyncTarget target, CancellationToken token = default)
        {
            var strategy = _strategies.FirstOrDefault(s => s.Handles(target));
            if (strategy == null) return;

            try
            {
                // Получаем все задачи для целевого хранилища
                var jobsToProcess = _pendingJobs
                    .Where(kvp => kvp.Key.Target == target)
                    .ToList();

                // Очищаем очередь для текущего целевого хранилища
                foreach (var (key, _) in jobsToProcess)
                {
                    _pendingJobs.TryRemove(key, out _);
                }

                // Обрабатываем все задачи пакетно
                foreach (var (_, job) in jobsToProcess)
                {
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
            }
            catch (OperationCanceledException)
            {
                Debug.Log($"Processing for target {target} canceled.");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Unexpected error in sync process: {ex.Message}");
            }
        }

        /// <summary>
        /// Освобождение ресурсов
        /// </summary>
        public void Dispose()
        {
            _cts.Cancel();
            _cts.Dispose();
            _pendingJobs.Clear();
            Debug.Log("SyncManager disposed");
        }
    }
}