// DataServiceBuilder.cs (в папке Services)

using System.Collections.Generic;
using _Project.System.DS.Core.Storage;
using DS.Configs;
using DS.Core.Cache;
using DS.Core.Interfaces;
using DS.Core.Sync;
using DS.Core.Sync.Strategies;
using DS.Models;

namespace DS.Services
{
    public class DataServiceBuilder 
    {
        private DSConfig _config;
        private SyncScheduler _scheduler;

        public DataServiceBuilder WithConfig(DSConfig config) 
        {
            _config = config;
            return this;
        }

        public DataService Build() 
        {
            // 1. Создаем кэш
            var cache = new MemoryCacheStorage();

            // 2. Создаем локальное хранилище
            ILocalStorage localStorage = new JsonStorage(_config.LocalStoragePath);

            // 3. Создаем удаленное хранилище
            IRemoteStorage remoteStorage = !string.IsNullOrEmpty(_config.RemoteApiUrl)
                ? new RestStorage(_config.RemoteApiUrl, _config.AuthToken)
                : new MockRemoteStorage();

            // 4. Создаем стратегии синхронизации
            var strategies = new List<ISyncStrategy> 
            {
                new LocalSync(localStorage)
            };
        
            if (!string.IsNullOrEmpty(_config.RemoteApiUrl)) 
            {
                strategies.Add(new RemoteSync(remoteStorage));
            }

            // 5. Создаем менеджер синхронизации
            var syncManager = new SyncManager(strategies.ToArray());
            _scheduler = new SyncScheduler(syncManager, new SyncSettings 
            {
                LocalInterval = _config.LocalSyncInterval,
                RemoteInterval = _config.RemoteSyncInterval
            });
            // 6. Возвращаем DataService
            return new DataService(cache, syncManager, localStorage, remoteStorage);
        }
        public SyncScheduler GetScheduler() => _scheduler;
    }
}