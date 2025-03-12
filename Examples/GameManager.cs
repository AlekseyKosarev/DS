using System;
using System.IO;
using System.Threading.Tasks;
using DS.Configs;
using DS.Core.Sync;
using DS.Core.Utils;
using DS.Services;
using UnityEngine;

namespace DS.Examples
{
    public class GameManager : MonoBehaviour {
        private DataService _dataService;
        private PlayerData _player;
        private SyncScheduler _syncScheduler;
        public DataMonitorUI dataMonitorUI;


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
            dataMonitorUI.Init(_dataService);
            // Загрузка данных с обработкой результата
            var loadResult = await _dataService.LoadAllAsync<PlayerData>("playerdata");
            if (loadResult.IsSuccess) {
                Debug.Log("данные загружены");
                _player = loadResult.Data[0];
            } else {
                Debug.Log("данные не загружены");
                _player = new PlayerData { playerName = "NewPlayer", level = 1, health = 100 };
                await SavePlayerData();
            }
        }

        DataService InitDS() 
        {
            var config = new DSConfig {
                LocalSyncInterval = TimeSpan.FromSeconds(5),
                RemoteSyncInterval = TimeSpan.FromSeconds(5),
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
            var saveResult = await _dataService.SaveAsync(KeyNamingRules.PlayerData("1"), _player);
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