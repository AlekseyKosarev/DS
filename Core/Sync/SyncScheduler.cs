using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DS.Core.Enums;
using DS.Models;
using UnityEngine;

namespace DS.Core.Sync
{
    public class SyncScheduler : IDisposable
    {
        private readonly CancellationTokenSource _cts = new();
        private readonly SyncSettings _settings;
        private readonly SyncManager _syncManager;

        public SyncScheduler(SyncManager syncManager, SyncSettings settings)
        {
            _syncManager = syncManager;
            _settings = settings;
            StartLocalSync();
            StartRemoteSync();
        }

        public void Dispose()
        {
            _cts.Cancel();
            _cts.Dispose();
        }

        private void StartLocalSync()
        {
            UniTask.Create(async () =>
            {
                while (!_cts.IsCancellationRequested)
                {
                    await UniTask.Delay(_settings.LocalInterval, cancellationToken: _cts.Token);
                    var result = await _syncManager.ProcessQueueAsync(SyncTarget.Local, _cts.Token);
                    if (!result.IsSuccess)
                        Debug.LogError(result.ErrorMessage);
                }
            }).Forget();
        }

        private void StartRemoteSync()
        {
            UniTask.Create(async () =>
            {
                while (!_cts.IsCancellationRequested)
                {
                    await UniTask.Delay(_settings.RemoteInterval, cancellationToken: _cts.Token);
                    var result = await _syncManager.ProcessQueueAsync(SyncTarget.Remote, _cts.Token);
                    // if (!result.IsSuccess)
                    //     Debug.LogError(result.ErrorMessage);
                }
            }).Forget();
        }
    }
}