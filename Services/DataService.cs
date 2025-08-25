using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DS.Core.Enums;
using DS.Core.Interfaces;
using DS.Core.Sync;
using DS.Examples.Data;
using DS.Models;
using UnityEngine;

namespace DS.Services
{
    public class DataService : IDisposable
    {
        private readonly IStorage _cacheStorage;
        private readonly IStorage _localStorage;
        private readonly IStorage _remoteStorage;
        private readonly SyncManager _syncManager;
        private readonly SyncScheduler _syncScheduler;

        public DataService(IStorage cacheStorage, IStorage localStorage, IStorage remoteStorage,
            SyncManager syncManager, SyncScheduler syncScheduler)
        {
            _cacheStorage = cacheStorage;
            _localStorage = localStorage;
            _remoteStorage = remoteStorage;
            _syncManager = syncManager;
            _syncScheduler = syncScheduler;
        }

        public void Dispose()
        {
            // Освобождаем ресурсы
            _syncScheduler?.Dispose();
            (_syncManager as IDisposable)?.Dispose();
            _cacheStorage?.Dispose();
            _localStorage?.Dispose();
            _remoteStorage?.Dispose();
        }

        public UniTask<Result<T>> SaveAsync<T>(string key, T data, CancellationToken token = default)
            where T : DataEntity
        {
            try
            {
                data.Version++;
                data.LastModified = DateTime.UtcNow;

                _cacheStorage.Save(key, data, token);

                _syncManager.AddJobInQueue(SyncTarget.Local, new SyncJob(key, data));
                _syncManager.AddJobInQueue(SyncTarget.Remote, new SyncJob(key, data));

                return UniTask.FromResult(Result<T>.Success(data));//TODO переделать на waitFoAll
            }
            catch (Exception ex)
            {
                return UniTask.FromResult(Result<T>.Failure($"Save failed: {ex.Message}"));
            }
        }

        public void SyncForced(SyncTarget target)
        {
            _syncManager.ProcessQueueAsync(target).Forget();
            // _syncScheduler.SyncForced(); //TODO change this func later - upd timers and other logic...
        }
        
        public async UniTask<Result<T>> LoadAsync<T>(string key, StorageType source = StorageType.Cache,
            bool autoSave = true,
            bool checkNextStorage = true, CancellationToken token = default) where T : DataEntity
        {
            var data = await LoadAllAsync<T>(key, source, autoSave, checkNextStorage, token);
            if (data.IsSuccess)
            {
                return Result<T>.Success(data.Data[0]);
            }
            return Result<T>.Failure(data.ErrorMessage);
        }
        
        public async UniTask<Result<T[]>> LoadAllAsync<T>(string prefix, StorageType source = StorageType.Cache,
            bool autoSave = true,
            bool checkNextStorage = true, CancellationToken token = default) where T : DataEntity
        {
            switch (source)
            {
                case StorageType.Cache:
                    var resCache = await LoadData<T>(prefix, _cacheStorage, autoSave, token);
                    if (resCache.IsSuccess)
                        return resCache;
                    if (checkNextStorage)
                        goto case StorageType.Local;
                    break;

                case StorageType.Local:
                    var resLocal = await LoadData<T>(prefix, _localStorage, autoSave, token);
                    if (resLocal.IsSuccess)
                        return resLocal;
                    if (checkNextStorage)
                        goto case StorageType.Remote;
                    break;

                case StorageType.Remote:
                    var resRemote = await LoadData<T>(prefix, _remoteStorage, autoSave, token);
                    if (resRemote.IsSuccess)
                        return resRemote;
                    break;
            }

            return Result<T[]>.Failure("Data not found.");
        }

        private async UniTask<Result<T[]>> LoadData<T>(string prefix, IStorage source, bool autoSave = true,
            CancellationToken token = default) where T : DataEntity
        {
            var keys = await source.GetKeysForPrefix(prefix, token);
            var result = await source.LoadAll<T>(keys, token);
            if (result.IsSuccess)
            {
                if (autoSave)
                {
                    if (source.GetType() == _localStorage.GetType())
                    {
                        _cacheStorage.SaveAll(keys, result.Data, token).Forget();
                        Debug.Log("_localStorage: " + result.Data.Length);
                    }

                    if (source.GetType() == _remoteStorage.GetType())
                    {
                        _cacheStorage.SaveAll(keys, result.Data, token).Forget();
                        _syncManager.AddJobsInQueue(SyncTarget.Local, keys, result.Data);
                        Debug.Log("_remoteStorage: " + result.Data.Length);
                    }
                }

                return result;
            }

            return Result<T[]>.Failure("Data not found.");
        }
    }
}