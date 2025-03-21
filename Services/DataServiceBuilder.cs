// DataServiceBuilder.cs (в папке Services)

using System;
using System.Collections.Generic;
using DS.Configs;
using DS.Core.Interfaces;
using DS.Core.Storage;
using DS.Core.Storage.Cache;
using DS.Core.Sync;
using DS.Core.Sync.Strategies;
using DS.Models;
using UnityEngine;

namespace DS.Services
{
    public class DataServiceBuilder
    {
        private IStorage _cacheStorage;
        private DSConfig _config;
        private IStorage _localStorage;
        private IStorage _remoteStorage;

        public DataServiceBuilder WithConfig(DSConfig config)
        {
            _config = config;
            return this;
        }

        public DataServiceBuilder ChangeTypesStorages<TCache, TLocal, TRemote>()
            where TCache : IStorage where TLocal : IStorage where TRemote : IStorage
        {
            if (typeof(TCache) == typeof(MemoryCacheStorage))
                _cacheStorage = new MemoryCacheStorage();

            if (typeof(TLocal) == typeof(JsonStorage))
                _localStorage = new JsonStorage(_config.LocalStoragePath);

            if (typeof(TRemote) == typeof(MockRemoteStorage))
                _remoteStorage = new MockRemoteStorage();

            if (typeof(TRemote) == typeof(RestStorage))
                _remoteStorage = new RestStorage(_config.RemoteApiUrl, _config.AuthToken);

            return this;
        }

        public DataService Build()
        {
            if (_cacheStorage == null) throw new ArgumentNullException(nameof(_localStorage));
            if (_localStorage == null) throw new ArgumentNullException(nameof(_localStorage));
            if (_remoteStorage == null) throw new ArgumentNullException(nameof(_localStorage));

            // 4. Создаем стратегии синхронизации
            var localStrategy = new LocalSync(_localStorage);
            var remoteStrategy = new RemoteSync(_remoteStorage);

            var strategies = new List<ISyncStrategy> { localStrategy, remoteStrategy };
            Debug.Log("strategies count: " + strategies.Count);

            // 5. Создаем менеджер синхронизации
            var syncManager = new SyncManager(strategies.ToArray());
            var syncScheduler = new SyncScheduler(syncManager, new SyncSettings
            {
                LocalInterval = _config.LocalSyncInterval,
                RemoteInterval = _config.RemoteSyncInterval
            });

            return new DataService(_cacheStorage, _localStorage, _remoteStorage, syncManager, syncScheduler);
        }
    }
}