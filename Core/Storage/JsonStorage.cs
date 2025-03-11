using System;
using System.IO;
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