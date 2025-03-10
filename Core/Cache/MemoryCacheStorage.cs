using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using DS.Core.Interfaces;
using DS.Models;

namespace DS.Core.Cache
{
    public class MemoryCacheStorage : ICacheStorage {
        private readonly ConcurrentDictionary<string, CacheEntry> _cache = new();
        private readonly AsyncTimer _cleanupTimer;
        private readonly int _maxSize; // Сохраняем maxSize
        private readonly TimeSpan _defaultTTL; // Сохраняем глобальный TTL
        public MemoryCacheStorage(int maxSize, TimeSpan ttl) {
            _maxSize = maxSize;
            _defaultTTL = ttl;
            _cleanupTimer = new AsyncTimer();
            _cleanupTimer.Start(TimeSpan.FromMinutes(1), async token => {
                await CleanupAsync();
            });
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
                // Используем ttl из параметра метода, но можно также использовать _defaultTTL
                var entry = new CacheEntry(data, ttl == default ? _defaultTTL : ttl);
            
                // Проверяем размер кэша
                if (_cache.Count >= _maxSize) {
                    RemoveOldest(); // Удаляем старые элементы
                }

                _cache.AddOrUpdate(key, entry, (k, old) => entry);
                onComplete?.Invoke();
            } catch (Exception ex) {
                onError?.Invoke(ex);
            }
        }

        public void Remove(string key) => _cache.TryRemove(key, out _);
        private async UniTask CleanupAsync() {
            // Логика очистки кэша
        }
        private void Cleanup(object state) {
            var now = DateTime.UtcNow;
            foreach (var key in _cache.Keys.ToList()) {
                if (_cache.TryGetValue(key, out var entry)) {
                    if (entry.IsExpired() || _cache.Count > _maxSize) {
                        _cache.TryRemove(key, out _);
                    }
                }
            }
        }
        // Метод для удаления старых элементов по LRU
        private void RemoveOldest() {
            if (_cache.IsEmpty) return;

            var oldestKey = _cache
                .OrderBy(kvp => kvp.Value.LastAccess)
                .FirstOrDefault().Key;

            _cache.TryRemove(oldestKey, out _);
        }
        public void Dispose() {
            _cleanupTimer?.Dispose();
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