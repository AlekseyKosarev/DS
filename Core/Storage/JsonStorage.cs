using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using DS.Core.Interfaces;
using DS.Models;
using Newtonsoft.Json;

namespace DS.Core.Storage
{
    public class JsonStorage : IStorage
    {
        private readonly string _storagePath;

        public JsonStorage(string storagePath)
        {
            _storagePath = storagePath;
            Directory.CreateDirectory(_storagePath);
        }

        public async UniTask<Result> Save(string key, DataEntity data, CancellationToken token = default)
        {
            try
            {
                var path = GetPath(key);
                var json = JsonConvert.SerializeObject(data);
                await File.WriteAllTextAsync(path, json, token);
                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure($"Save failed: {ex.Message}");
            }
        }

        public async UniTask<Result[]> SaveAll(string[] keys, DataEntity[] data, CancellationToken token = default)
        {
            var tasks = new List<UniTask<Result>>();
            for(var i = 0; i < keys.Length; i++)
            {
                tasks.Add(Save(keys[i], data[i], token));
            }
            
            return await UniTask.WhenAll(tasks);
        }

        public async UniTask<Result<T>> Load<T>(string key, CancellationToken token = default) where T : DataEntity
        {
            try
            {
                var path = GetPath(key);
                if (!File.Exists(path)) return Result<T>.Failure("File not found.");

                var json = await File.ReadAllTextAsync(path, token);
                var data = JsonConvert.DeserializeObject<T>(json);
                return Result<T>.Success(data);
            }
            catch (Exception ex)
            {
                return Result<T>.Failure($"Load failed: {ex.Message}");
            }
        }

        public async UniTask<Result<T[]>> LoadAll<T>(string[] keys, CancellationToken token = default) where T : DataEntity
        {
            try
            {
                if (keys == null || keys.Length == 0)
                    return Result<T[]>.Failure("LoadAll - keys array is null or empty.");

                var tasks = keys.Select(key => Load<T>(key, token));
                var results = await UniTask.WhenAll(tasks);

                var successfulResults = results
                    .Where(result => result.IsSuccess)
                    .Select(result => result.Data)
                    .ToArray();
            
                return Result<T[]>.Success(successfulResults);
            }
            catch (Exception ex)
            {
                return Result<T[]>.Failure($"LoadAll failed: {ex.Message}");
            }
            
        }

        public async UniTask<Result<T[]>> LoadAllForPrefix<T>(string prefix, CancellationToken token = default)
            where T : DataEntity
        {
            try
            {
                var keys = await GetKeysForPrefix(prefix, token);
                return await LoadAll<T>(keys, token); 
            }
            catch (Exception ex)
            {
                return Result<T[]>.Failure($"LoadAllPrefix failed: {ex.Message}");
            }
        }

        public UniTask<string[]> GetKeysForPrefix(string prefix, CancellationToken token = default)
        {
            try
            {
                var files = Directory.GetFiles(_storagePath, "*.json");
                var fileNames = files
                    .Select(file => Path.GetFileNameWithoutExtension(file))
                    .Where(fileName => string.IsNullOrEmpty(prefix) || fileName.StartsWith(prefix)).ToArray();

                return UniTask.FromResult(fileNames);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to get local keys: {ex.Message}");
            }
        }

        public UniTask<Result> Delete(string key, CancellationToken token = default)
        {
            try
            {
                var path = GetPath(key);
                if (File.Exists(path)) File.Delete(path);
                return UniTask.FromResult(Result.Success());
            }
            catch (Exception ex)
            {
                return UniTask.FromResult(Result.Failure($"Delete failed: {ex.Message}"));
            }
        }

        public UniTask<Result> DeleteAllForPrefix(string prefix, CancellationToken token = default)
        {
            try
            {
                var keys = GetKeysForPrefix(prefix, token).GetAwaiter().GetResult();

                foreach (var key in keys) Delete(key, token).GetAwaiter().GetResult();

                return UniTask.FromResult(Result.Success());
            }
            catch (Exception ex)
            {
                return UniTask.FromResult(Result.Failure($"DeleteAll failed: {ex.Message}"));
            }
        }

        public void Dispose()
        {
            // Debug.Log("JsonStorage disposed.");
        }

        private string GetPath(string key)
        {
            return Path.Combine(_storagePath, $"{key}.json");
        }
    }
}