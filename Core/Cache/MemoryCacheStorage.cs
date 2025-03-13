using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using DS.Core.Interfaces;
using DS.Models;
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

        public void Set(string key, DataEntity data, Action onComplete = null, Action<Exception> onError = null)
        {
            try {
                _cache[key] = data;
                onComplete?.Invoke();
            } 
            catch (Exception ex) {
                onError?.Invoke(ex);
            }
        }

        public Result<T[]> GetAll<T>(string[] keys, Action<T[]> onComplete = null, Action<Exception> onError = null) where T : DataEntity
        {
            try
            {
                if (keys == null || keys.Length == 0)
                {
                    onError?.Invoke(new ArgumentException("Keys array is null or empty."));
                    return Result<T[]>.Failure("not found.");
                }

                var results = new List<T>();

                foreach (var key in keys)
                {
                    var data = _cache.TryGetValue(key, out var entity) ? entity as T : null;
                    if (data != null)
                    {
                        results.Add(data);
                    }
                }
                onComplete?.Invoke(results.ToArray());
                return Result<T[]>.Success(results.ToArray());
            }
            catch (Exception ex)
            {
                onError?.Invoke(ex);
                return Result<T[]>.Failure(ex.Message);
            }
        }

        public string[] GetCacheKeys(string prefix = null)
        {
            return _cache.Keys
                .Where(key => string.IsNullOrEmpty(prefix) || key.StartsWith(prefix)).ToArray();
        }

        public void Remove(string key) => _cache.TryRemove(key, out _);
        public void Clear() => _cache.Clear();
        public void Dispose()
        {
            Clear();
        }
    }
}