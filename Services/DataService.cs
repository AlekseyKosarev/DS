using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using DS.Core.Enums;
using DS.Core.Interfaces;
using DS.Core.Sync;
using DS.Core.Utils;
using DS.Models;

namespace DS.Services
{
    public class DataService: IDisposable {
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

        public UniTask<Result<T>> SaveAsync<T>(string key, T data, CancellationToken token = default) where T : DataEntity {
            try {
                data.Version++;
                data.LastModified = DateTime.UtcNow;

                _cacheStorage.Set(key, data);

                _syncManager.Queue(SyncTarget.Local, new SyncJob(key, data));
                _syncManager.Queue(SyncTarget.Remote, new SyncJob(key, data));

                return UniTask.FromResult(Result<T>.Success(data));
            } catch (Exception ex) {
                return UniTask.FromResult(Result<T>.Failure($"Save failed: {ex.Message}"));
            }
        }

        public async UniTask<Result<T>> LoadAsync<T>(string key, CancellationToken token = default) where T : DataEntity {
            try {
                var cached = _cacheStorage.Get<T>(key);
                if (cached != null) return Result<T>.Success(cached);

                // Загрузка из локального хранилища
                var local = await _localStorage.LoadAsync<T>(key, token);
                if (local.IsSuccess) {
                    _cacheStorage.Set(key, local.Data);
                    return local;
                }

                // Загрузка из удаленного хранилища
                var remote = await _remoteStorage.DownloadAsync<T>(key, token);
                if (remote.IsSuccess) {
                    _cacheStorage.Set(key, remote.Data);
                    _syncManager.Queue(SyncTarget.Local, new SyncJob(key, remote.Data));
                    return remote;
                }

                return Result<T>.Failure("Data not found.");
            } catch (Exception ex) {
                return Result<T>.Failure($"Load failed: {ex.Message}");
            }
        }
        public async UniTask<Result<T[]>> LoadAllAsync<T>(string prefix, StorageEnum source = StorageEnum.Auto, CancellationToken token = default) where T : DataEntity
        {
            string[] keys;
            if (source == StorageEnum.Auto)
                source = StorageEnum.Cache;
            
            switch (source)
            {
                case StorageEnum.Cache:
                    keys = _cacheStorage.GetCacheKeys(prefix);
                    var resCache = _cacheStorage.GetAll<T>(keys);
                    if (resCache.IsSuccess)
                        return resCache;
                    goto case StorageEnum.Local;

                case StorageEnum.Local:
                    keys = await _localStorage.GetLocalKeysAsync(prefix, token);
                    var resLocal =  await _localStorage.LoadAllAsync<T>(keys.ToArray(), token);
                    if (resLocal.IsSuccess)
                        return resLocal;
                    goto case StorageEnum.Remote;

                case StorageEnum.Remote:
                    keys = await _remoteStorage.GetRemoteKeysAsync(prefix, token);
                    var resRemote = await _remoteStorage.DownloadAllAsync<T>(keys, token);
                    if (resRemote.IsSuccess)
                        return resRemote;
                    break;
            }

            return Result<T[]>.Failure("Data not found.");
        }
        
        // Получение данных из кэша
        public T GetFromCache<T>(string key) where T : DataEntity {
            return _cacheStorage.Get<T>(key);
        }
        // Асинхронное получение из локального хранилища
        public async UniTask<Result<T>> GetFromLocalStorageAsync<T>(string key, CancellationToken token = default) 
            where T : DataEntity 
        {
            try {
                return await _localStorage.LoadAsync<T>(key, token);
            } catch (Exception ex) {
                return Result<T>.Failure($"Local load failed: {ex.Message}");
            }
        }
        // Асинхронное получение из удаленного хранилища
        public async UniTask<Result<T>> GetFromRemoteStorageAsync<T>(string key, CancellationToken token = default) 
            where T : DataEntity 
        {
            try {
                return await _remoteStorage.DownloadAsync<T>(key, token);
            } catch (Exception ex) {
                return Result<T>.Failure($"Remote load failed: {ex.Message}");
            }
        }
        
        public async UniTask<DebugDataSnapshot<T>> GetDebugSnapshotAsync<T>(string key, CancellationToken token = default) 
            where T : DataEntity 
        {
            var snapshot = new DebugDataSnapshot<T> {
                CacheData = GetFromCache<T>(key),
                LocalData = await GetFromLocalStorageAsync<T>(key, token),
                RemoteData = await GetFromRemoteStorageAsync<T>(key, token)
            };
            return snapshot;
        }
        public void ClearCache(string key) {
            _cacheStorage.Remove(key);
        }
        public void ClearCache() {
            _cacheStorage.Clear();
        }
        public void DeleteLocalData(string key) {
            _localStorage.DeleteAsync(key);
            _cacheStorage.Remove(key);
        }

        public void Dispose() {
            // Освобождаем ресурсы
            _cacheStorage?.Dispose();
            (_syncManager as IDisposable)?.Dispose();
            _localStorage?.Dispose();
            _remoteStorage?.Dispose();
        }
    }
}