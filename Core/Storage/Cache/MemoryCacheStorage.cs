using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using DS.Core.Interfaces;
using DS.Models;

namespace DS.Core.Storage.Cache
{
    public class MemoryCacheStorage : IStorage
    {
        private readonly ConcurrentDictionary<string, DataEntity> _cache = new();

        public async UniTask<Result> Save(string key, DataEntity data, CancellationToken token = default)
        {
            try
            {
                _cache[key] = data;
                return await UniTask.FromResult(Result.Success());
            }
            catch (Exception ex)
            {
                return await UniTask.FromResult(Result.Failure($"Save failed: {ex.Message}"));
            }
        }

        public async UniTask<Result[]> SaveAll(string[] keys, DataEntity[] data, CancellationToken token = default)
        {
            var tasks = new List<UniTask<Result>>();
            for (var i = 0; i < keys.Length; i++) tasks.Add(Save(keys[i], data[i], token));

            return await UniTask.WhenAll(tasks);
        }

        public async UniTask<Result<T>> Load<T>(string key, CancellationToken token = default)
            where T : DataEntity
        {
            if (_cache.TryGetValue(key, out var data)) return await UniTask.FromResult(Result<T>.Success(data as T));
            return await UniTask.FromResult(Result<T>.Failure("not found."));
        }

        public async UniTask<Result<T[]>> LoadAll<T>(string[] keys, CancellationToken token = default)
            where T : DataEntity
        {
            try
            {
                if (keys == null || keys.Length == 0)
                    return await UniTask.FromResult(Result<T[]>.Failure("not found."));

                var results = new List<T>();
                foreach (var key in keys)
                {
                    var result = Load<T>(key, token).GetAwaiter().GetResult();
                    if (result.IsSuccess) results.Add(result.Data);
                }

                return await UniTask.FromResult(Result<T[]>.Success(results.ToArray()));
            }
            catch (Exception ex)
            {
                return await UniTask.FromResult(Result<T[]>.Failure("Cache LoadAll failed: " + ex.Message));
            }
        }

        public async UniTask<Result<T[]>> LoadAllForPrefix<T>(string prefix, CancellationToken token = default)
            where T : DataEntity
        {
            try
            {
                var keys = GetKeysForPrefix(prefix, token).GetAwaiter().GetResult();
                return await LoadAll<T>(keys, token);
            }
            catch (Exception ex)
            {
                return await UniTask.FromResult(Result<T[]>.Failure("Cache LoadAllForPrefix failed: " + ex.Message));
            }
        }

        public UniTask<string[]> GetKeysForPrefix(string prefix = null, CancellationToken token = default)
        {
            var keys = _cache.Keys
                .Where(key =>
                    string.IsNullOrEmpty(prefix) || key.StartsWith(prefix)).ToArray();
            return UniTask.FromResult(keys);
        }

        public UniTask<Result> Delete(string key, CancellationToken token = default)
        {
            if (_cache.TryRemove(key, out _))
                return UniTask.FromResult(Result.Success());
            return UniTask.FromResult(Result.Failure("not found."));
        }

        public UniTask<Result> DeleteAllForPrefix(string prefix, CancellationToken token = default)
        {
            var keys = GetKeysForPrefix(prefix, token).GetAwaiter().GetResult();

            foreach (var key in keys) Delete(key, token).GetAwaiter().GetResult();
            return UniTask.FromResult(Result.Success()); //TODO возможно потом лучше добавить и Failure
        }

        public void Dispose()
        {
            Clear();
        }

        public void Clear()
        {
            _cache.Clear();
        }
    }
}