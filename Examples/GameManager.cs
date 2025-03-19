using System;
using System.IO;
using Cysharp.Threading.Tasks;
using DS.Configs;
using DS.Core.Storage;
using DS.Core.Storage.Cache;
using DS.Examples.Data;
using DS.Services;
using DS.Utils;
using UnityEngine;

namespace DS.Examples
{
    public class GameManager : MonoBehaviour
    {
        private PlayerData _player;
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
                RemoteSyncInterval = TimeSpan.FromSeconds(5),
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
            LoadPlayerData().Forget();
        }

        private async UniTask LoadPlayerData()
        {
            var loadResult = await Ds.LoadAllAsync<PlayerData>(KeyNamingRules.KeyFor<PlayerData>());

            if (loadResult.IsSuccess)
            {
                Debug.Log("данные загружены");
                _player = loadResult.Data[0];
            }
            else
            {
                Debug.Log("данные не загружены. Создаем нового игрока");
                _player = new PlayerData
                {
                    id = 1,
                    name = "NewPlayer",
                    level = 1
                };
                await SavePlayerData();
            }
        }

        public async UniTask SavePlayerData()
        {
            var saveResult = await Ds.SaveAsync(KeyNamingRules.KeyFor<PlayerData>(), _player);
            if (!saveResult.IsSuccess)
                Debug.LogError($"Failed to save player data: {saveResult.ErrorMessage}");
        }

        public void UpgradePlayerData()
        {
            _player.level++;
            _ = SavePlayerData();
        }
    }
}