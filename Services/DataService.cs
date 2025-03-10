using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DS.Core.Enums;
using DS.Core.Interfaces;
using DS.Core.Sync;
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

                _cacheStorage.Set(key, data, TimeSpan.FromMinutes(10));

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
                    _cacheStorage.Set(key, local.Data, TimeSpan.FromMinutes(10));
                    return local;
                }

                // Загрузка из удаленного хранилища
                var remote = await _remoteStorage.DownloadAsync<T>(key, token);
                if (remote.IsSuccess) {
                    _cacheStorage.Set(key, remote.Data, TimeSpan.FromMinutes(10));
                    _syncManager.Queue(SyncTarget.Local, new SyncJob(key, remote.Data));
                    return remote;
                }

                return Result<T>.Failure("Data not found.");
            } catch (Exception ex) {
                return Result<T>.Failure($"Load failed: {ex.Message}");
            }
        }
        
        public void ClearCache(string key) {
            _cacheStorage.Remove(key);
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