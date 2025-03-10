using System;
using DS.Core.Cache;
using DS.Core.Enums;
using DS.Core.Interfaces;
using DS.Core.Storage;
using DS.Core.Sync;
using DS.Core.Sync.Strategies;
using DS.Models;
using DS.Utilites;

namespace DS.Services
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
        
        public void ClearCache(string key) {
            _cacheStorage.Remove(key);
        }

        public void DeleteLocalData(string key) {
            _localStorage.Delete(key);
            _cacheStorage.Remove(key);
        }
    }
}