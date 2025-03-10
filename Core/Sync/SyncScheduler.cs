using System.Threading;

namespace _Project.System.DataManagementService
{
    public class SyncScheduler {
        private readonly SyncManager _syncManager;
        private readonly SyncSettings _settings;
        private readonly Timer _localTimer;
        private readonly Timer _remoteTimer;

        public SyncScheduler(SyncManager syncManager, SyncSettings settings) {
            _syncManager = syncManager;
            _settings = settings;
            _localTimer = new Timer(_ => _syncManager.ProcessQueue(SyncTarget.Local), null, _settings.LocalInterval, _settings.LocalInterval);
            _remoteTimer = new Timer(_ => _syncManager.ProcessQueue(SyncTarget.Remote), null, _settings.RemoteInterval, _settings.RemoteInterval);
        }
    }
}