using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using DS.Core.Interfaces;
using DS.Models;
using DS.Utilites;
using UnityEngine;

namespace DS.Core.Cache
{
    public class MemoryCacheStorage : ICacheStorage 
    {
        private readonly ConcurrentDictionary<string, DataEntity> _cache = new();
        
        public T Get<T>(string key) where T : DataEntity 
        {
            if (_cache.TryGetValue(key, out var data))
            {
                return (T)data;
            }
            return null;
        }

        public void Set(string key, DataEntity data, TimeSpan ttl, Action onComplete = null, Action<Exception> onError = null)
        {
            try {
                _cache[key] = data;
                onComplete?.Invoke();
            } 
            catch (Exception ex) {
                onError?.Invoke(ex);
            }
        }

        public void Remove(string key) => _cache.TryRemove(key, out _);

        public void Dispose() 
        {
            _cache.Clear();
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