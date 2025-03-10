using System;
using System.IO;
using System.Threading.Tasks;
using DS.Configs;
using DS.Core.Sync;
using DS.Services;
using UnityEngine;

namespace DS.Examples
{
    public class GameManager : MonoBehaviour {
        private DataService _dataService;
        private ExampleData _player;
        private SyncScheduler _syncScheduler;


        public void InitClick()
        {
            _ = Init();
        }

        public void SaveClick()
        {
            _ = SavePlayerData();
        }
        public async Task Init() {
            _dataService = InitDS();

            // Загрузка данных с обработкой результата
            var loadResult = await _dataService.LoadAsync<ExampleData>("player");
            if (loadResult.IsSuccess) {
                _player = loadResult.Data;
            } else {
                _player = new ExampleData { playerName = "NewPlayer", level = 1, health = 100 };
                await SavePlayerData();
            }
        }

        DataService InitDS() 
        {
            var config = new DSConfig {
                LocalSyncInterval = TimeSpan.FromSeconds(10),
                RemoteSyncInterval = TimeSpan.FromMinutes(5),
                CacheMaxSize = 500,
                CacheTTL = TimeSpan.FromMinutes(5),
                LocalStoragePath = Path.Combine(
                    Application.dataPath, 
                    "_Game", "_Saves"
                )
            };
            var builder = new DataServiceBuilder()
                .WithConfig(config);
            var dataService = builder.Build();
            _syncScheduler = builder.GetScheduler();
            return dataService;
        }

        public async Task SavePlayerData() {
            var saveResult = await _dataService.SaveAsync("player", _player);
            if (!saveResult.IsSuccess) {
                Debug.LogError($"Failed to save player data: {saveResult.ErrorMessage}");
            }
        }

        public void UpgradeLevel() {
            _player.level++;
            _ = SavePlayerData();
        }

        void OnDestroy() {
            _syncScheduler?.Dispose();
            (_dataService as IDisposable)?.Dispose();
        }
    }
}