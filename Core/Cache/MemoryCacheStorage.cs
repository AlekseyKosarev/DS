using System;
using System.Collections.Concurrent;
using System.Threading;

namespace _Project.System.DataManagementService
{
    public class MemoryCacheStorage : ICacheStorage {
        private readonly ConcurrentDictionary<string, CacheEntry> _cache = new();
        private readonly Timer _cleanupTimer;

        public MemoryCacheStorage() {
            _cleanupTimer = new Timer(Cleanup, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
        }

        public T Get<T>(string key) where T : DataEntity {
            if (_cache.TryGetValue(key, out var entry) && !entry.IsExpired()) {
                entry.Refresh();
                return (T)entry.Data;
            }
            return null;
        }

        public void Set(string key, DataEntity data, TimeSpan ttl, Action onComplete = null, Action<Exception> onError = null) {
            try {
                var entry = new CacheEntry(data, ttl);
                _cache.AddOrUpdate(key, entry, (k, old) => entry);
                onComplete?.Invoke();
            } catch (Exception ex) {
                onError?.Invoke(ex);
            }
        }

        public void Remove(string key) => _cache.TryRemove(key, out _);

        private void Cleanup(object state) {
            foreach (var key in _cache.Keys) {
                if (_cache.TryGetValue(key, out var entry) && entry.IsExpired()) {
                    _cache.TryRemove(key, out _);
                }
            }
        }

        private class CacheEntry {
            public DataEntity Data { get; }
            public DateTime ExpiryTime { get; }
            public DateTime LastAccess { get; private set; }

            public CacheEntry(DataEntity data, TimeSpan ttl) {
                Data = data;
                ExpiryTime = DateTime.UtcNow.Add(ttl);
                LastAccess = DateTime.UtcNow;
            }

            public bool IsExpired() => DateTime.UtcNow >= ExpiryTime;
            public void Refresh() => LastAccess = DateTime.UtcNow;
        }
    }
}