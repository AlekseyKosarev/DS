using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using DS.Core.Interfaces;
using DS.Models;
using Newtonsoft.Json;
using UnityEngine;

namespace _Project.System.DS.Core.Storage
{
    public class JsonStorage : ILocalStorage {
        private readonly string _storagePath;

        public JsonStorage(string storagePath) {
            _storagePath = storagePath;
            Directory.CreateDirectory(_storagePath);
        }

        private string GetPath(string key) {
            return Path.Combine(_storagePath, $"{key}.json");
        }

        public async UniTask<Result> SaveAsync(string key, object data, CancellationToken token = default) {
            try {
                var path = GetPath(key);
                var json = JsonConvert.SerializeObject(data);
                await File.WriteAllTextAsync(path, json, token);
                return Result.Success();
            } catch (Exception ex) {
                return Result.Failure($"Save failed: {ex.Message}");
            }
        }

        public async UniTask<Result<T>> LoadAsync<T>(string key, CancellationToken token = default) where T : DataEntity {
            try {
                var path = GetPath(key);
                if (!File.Exists(path)) return Result<T>.Failure("File not found.");

                var json = await File.ReadAllTextAsync(path, token);
                var data = JsonConvert.DeserializeObject<T>(json);
                return Result<T>.Success(data);
            } catch (Exception ex) {
                return Result<T>.Failure($"Load failed: {ex.Message}");
            }
        }

        public async UniTask<Result<T[]>> LoadAllAsync<T>(string[] keys, CancellationToken token = default) where T : DataEntity
        {
            try
            {
                Debug.Log(keys + " " + keys.Length);
                if (keys == null || keys.Length == 0)
                {
                    Debug.Log("fail = 0");
                    return Result<T[]>.Failure($"LoadAll - keys array is null or empty.");
                    //return Result<T[]>.Success(Array.Empty<T>());
                }

                var tasks = keys.Select(key => LoadAsync<T>(key, token));
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

        public async UniTask<string[]> GetLocalKeysAsync(string prefix = null, CancellationToken token = default)
        {
            try
            {
                var files = Directory.GetFiles(_storagePath, "*.json");
                return files
                    .Select(file => Path.GetFileNameWithoutExtension(file))
                    .Where(fileName => string.IsNullOrEmpty(prefix) || fileName.StartsWith(prefix)).ToArray();
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to get local keys: {ex.Message}");
            }
        }

        public UniTask<Result> DeleteAsync(string key, CancellationToken token = default) {
            try {
                var path = GetPath(key);
                if (File.Exists(path)) {
                    File.Delete(path);
                }
                return UniTask.FromResult(Result.Success());
            } catch (Exception ex) {
                return UniTask.FromResult(Result.Failure($"Delete failed: {ex.Message}"));
            }
        }

        public void Dispose() 
        {
            // Debug.Log("JsonStorage disposed.");
        }
    }
    
}