using System;
using System.IO;
using UnityEngine;

namespace DS.Configs
{
    public class DSConfig {
        // Настройки кэша
        // public TimeSpan CacheTTL { get; set; } = TimeSpan.FromMinutes(10);
        // public int CacheMaxSize { get; set; } = 1000;

        // Локальное хранилище
        public string LocalStoragePath { get; set; } = Path.Combine(
            Application.persistentDataPath, "GameData"
        );
        public bool UseBinaryFormat { get; set; } = false;

        // Удаленное хранилище
        public string RemoteApiUrl { get; set; } = string.Empty;
        public string AuthToken { get; set; } = string.Empty;

        // Синхронизация
        public TimeSpan LocalSyncInterval { get; set; } = TimeSpan.FromMinutes(5);
        public TimeSpan RemoteSyncInterval { get; set; } = TimeSpan.FromHours(1);
    }
}