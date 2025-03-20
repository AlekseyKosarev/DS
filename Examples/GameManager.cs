using System;
using System.IO;
using DS.Configs;
using DS.Core.Storage;
using DS.Core.Storage.Cache;
using DS.Services;
using UnityEngine;

namespace DS.Examples
{
    public class GameManager : MonoBehaviour
    {
        public DataService Ds;

        private void Start()
        {
            Init();
        }

        private void OnDestroy()
        {
            (Ds as IDisposable)?.Dispose();
        }


        private DataService InitDS()
        {
            var config = new DSConfig
            {
                LocalSyncInterval = TimeSpan.FromSeconds(5),
                RemoteSyncInterval = TimeSpan.FromSeconds(10),
                LocalStoragePath = Path.Combine(
                    Application.dataPath,
                    "_Game", "_Saves"
                )
            };
            var dataService = new DataServiceBuilder()
                .WithConfig(config)
                .ChangeTypesStorages<MemoryCacheStorage, JsonStorage, MockRemoteStorage>()
                .Build();
            return dataService;
        }

        public void Init()
        {
            Ds = InitDS();
        }
    }
}