using System;
using System.Threading;
using DS.Core.Enums;
using DS.Models;
using DS.Utilites;

namespace DS.Core.Sync
{
    public class SyncScheduler: IDisposable {
        private readonly AsyncTimer _localTimer;
        private readonly AsyncTimer _remoteTimer;
        private readonly CancellationTokenSource _cts = new();
        public SyncScheduler(SyncManager syncManager, SyncSettings settings) {
            _localTimer = new AsyncTimer();
            _localTimer.Start(settings.LocalInterval, async token => {
                await syncManager.ProcessQueueAsync(SyncTarget.Local, _cts.Token);
            });

            _remoteTimer = new AsyncTimer();
            _remoteTimer.Start(settings.RemoteInterval, async token => {
                await syncManager.ProcessQueueAsync(SyncTarget.Remote, _cts.Token);
            });
        }

        public void Dispose() {
            _cts.Cancel(); // Отменяем все задачи
            _cts.Dispose();
            _localTimer?.Dispose();
            _remoteTimer?.Dispose();
        }
    }
}