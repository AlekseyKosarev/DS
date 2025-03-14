using System;
using System.IO;
using System.Linq;
using System.Threading;
using _Project.System.DS.Core.Interfaces;
using _Project.System.DS.Models;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;

namespace _Project.System.DS.Core.Storage
{
    public class JsonStorage : IStorage {
        private readonly string _storagePath;

        public JsonStorage(string storagePath) {
            _storagePath = storagePath;
            Directory.CreateDirectory(_storagePath);
        }

        private string GetPath(string key) {
            return Path.Combine(_storagePath, $"{key}.json");
        }

        public async UniTask<Result> Save(string key, DataEntity data, CancellationToken token = default) {
            try {
                var path = GetPath(key);
                var json = JsonConvert.SerializeObject(data);
                await File.WriteAllTextAsync(path, json, token);
                return Result.Success();
            } catch (Exception ex) {
                return Result.Failure($"Save failed: {ex.Message}");
            }
        }

        public async UniTask<Result<T>> Load<T>(string key, CancellationToken token = default) where T : DataEntity {
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

        public async UniTask<Result<T[]>> LoadAllForPrefix<T>(string prefix, CancellationToken token = default) where T : DataEntity
        {
            try
            {
                var keys = await GetKeysForPrefix(prefix, token);
                if (keys == null || keys.Length == 0)//TODO подумать куда убрать
                {
                    return Result<T[]>.Failure($"LoadAll - keys array is null or empty.");
                }

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

        public UniTask<Result> Delete(string key, CancellationToken token = default) {
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

        public UniTask<Result> DeleteAllForPrefix(string prefix, CancellationToken token = default)
        {
            try
            {
                
                var keys = GetKeysForPrefix(prefix, token).GetAwaiter().GetResult();
                
                foreach (var key in keys)
                {
                    Delete(key, token).GetAwaiter().GetResult();
                }

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
    }
    
}