using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DS.Core.Enums;
using DS.Models;
using DS.Utilites;
using UnityEngine;

namespace DS.Core.Sync
{
    public class SyncScheduler: IDisposable
    {
        private readonly CancellationTokenSource _cts = new();
        public SyncScheduler(SyncManager syncManager, SyncSettings settings) {
            // Локальная синхронизация
            UniTask.Create(async () => {
                while (!_cts.IsCancellationRequested)
                {
                    await UniTask.Delay(settings.LocalInterval, cancellationToken: _cts.Token);
                    await syncManager.ProcessQueueAsync(SyncTarget.Local, _cts.Token);
                }
            }).Forget();

            // Удаленная синхронизация
            UniTask.Create(async () => {
                while (!_cts.IsCancellationRequested) 
                {
                    await UniTask.Delay(settings.RemoteInterval, cancellationToken: _cts.Token);
                    await syncManager.ProcessQueueAsync(SyncTarget.Remote, _cts.Token);
                }
            }).Forget();
        }

        public void Dispose() {
            _cts.Cancel();
            _cts.Dispose();
        }
    }
}