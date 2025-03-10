using System;
using System.Threading;
using DS.Core.Enums;
using DS.Models;

namespace DS.Core.Sync
{
    public class SyncScheduler: IDisposable {
        private readonly AsyncTimer _localTimer;
        private readonly AsyncTimer _remoteTimer;

        public SyncScheduler(SyncManager syncManager, SyncSettings settings) {
            _localTimer = new AsyncTimer();
            _localTimer.Start(settings.LocalInterval, async token => {
                await syncManager.ProcessQueueAsync(SyncTarget.Local, token);
            });

            _remoteTimer = new AsyncTimer();
            _remoteTimer.Start(settings.RemoteInterval, async token => {
                await syncManager.ProcessQueueAsync(SyncTarget.Remote, token);
            });
        }

        public void Dispose() {
            _localTimer?.Dispose();
            _remoteTimer?.Dispose();
        }
    }
}