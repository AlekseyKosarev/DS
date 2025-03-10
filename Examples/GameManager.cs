using System;
using System.IO;
using _Project.System.DS.Services;
using UnityEngine;
using DS.Core;
using DS.Core.Cache;
using DS.Core.Storage;
using DS.Core.Sync;
using DS.Core.Sync.Strategies;
using DS.Models;
using DS.Services;
using DS.Utilites;

public class GameManager : MonoBehaviour {
    private DataService _dataService;
    private ExampleData _player;
    private SyncScheduler _syncScheduler;
    public void Init() {
        // // Инициализация системы
        // var cache = new MemoryCacheStorage();
        // var localStorage = new JsonStorage();
        // var remoteStorage = new MockRemoteStorage();
        // var syncManager = new SyncManager(
        //     new LocalSync(localStorage),
        //     new RemoteSync(remoteStorage, new RetryPolicy())
        // );
        // _dataService = new DataService(cache, syncManager, localStorage, remoteStorage);
        
        // Загрузка данных
        _dataService = InitDS();
        _player = _dataService.Load<ExampleData>("player");
        if (_player == null) {
            _player = new ExampleData { playerName = "NewPlayer", level = 1, health = 100 };
            SavePlayerData();
        }

        // Инициализация синхронизации

        // InitSync(syncManager);
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
    //
    // void InitSync(SyncManager syncManager)
    // {
    //     var syncSettings = new SyncSettings {
    //         LocalInterval = TimeSpan.FromSeconds(10),  // Каждые 30 секунд
    //         RemoteInterval = TimeSpan.FromMinutes(5)   // Каждые 5 минут
    //     };
    //
    //     _syncScheduler = new SyncScheduler(syncManager, syncSettings);
    // }

    public void SavePlayerData() {
        _dataService.Save("player", _player, 
            onComplete: () => Debug.Log("Save successful"),
            onError: ex => Debug.LogError($"Save error: {ex.Message}")
        );
    }

    // Пример обновления данных
    public void UpgradeLevel() {
        _player.level++;
        SavePlayerData();
    }
    void OnDestroy() {
        // Освобождаем ресурсы
        _syncScheduler?.Dispose();
    }
}