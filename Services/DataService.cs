using System;

namespace _Project.System.DataManagementService
{
    public class DataService {
        private readonly ICacheStorage _cacheStorage;
        private readonly SyncManager _syncManager;
        private readonly ILocalStorage _localStorage;
        private readonly IRemoteStorage _remoteStorage;

        public DataService(ICacheStorage cacheStorage, SyncManager syncManager, ILocalStorage localStorage, IRemoteStorage remoteStorage) {
            _cacheStorage = cacheStorage;
            _syncManager = syncManager;
            _localStorage = localStorage;
            _remoteStorage = remoteStorage;
        }

        public void Save<T>(string key, T data, Action onComplete = null, Action<Exception> onError = null) where T : DataEntity {
            data.Version++;
            data.LastModified = DateTime.UtcNow;

            _cacheStorage.Set(key, data, TimeSpan.FromMinutes(10), onComplete, onError);
            _syncManager.Queue(SyncTarget.Local, new SyncJob(key, data) {
                OnComplete = onComplete,
                OnError = onError
            });
            _syncManager.Queue(SyncTarget.Remote, new SyncJob(key, data) {
                OnComplete = onComplete,
                OnError = onError
            });
        }

        public T Load<T>(string key) where T : DataEntity {
            var cached = _cacheStorage.Get<T>(key);
            if (cached != null) return cached;

            var local = _localStorage.Load<T>(key);
            if (local != null) {
                _cacheStorage.Set(key, local, TimeSpan.FromMinutes(10));
                return local;
            }

            var remote = _remoteStorage.Download<T>(key);
            if (remote != null) {
                _cacheStorage.Set(key, remote, TimeSpan.FromMinutes(10));
                _syncManager.Queue(SyncTarget.Local, new SyncJob(key, remote));
                return remote;
            }

            return null;
        }
    }
}