using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using DS.Core.Interfaces;
using DS.Models;
using Newtonsoft.Json;
using UnityEngine;

namespace DS.Core.Storage
{
    public class RestStorage : IStorage
    {
        private readonly HttpClient _httpClient;

        public RestStorage(string apiUrl, string authToken)
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(apiUrl);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
        }

        public async UniTask<Result> Save(string key, DataEntity data, CancellationToken token = default)
        {
            try
            {
                var json = JsonConvert.SerializeObject(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"save/{key}", content, token);
                response.EnsureSuccessStatusCode();
                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure($"Upload failed: {ex.Message}");
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
                var response = await _httpClient.GetAsync($"load/{key}", token);
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<T>(json);
                return Result<T>.Success(data);
            }
            catch (Exception ex)
            {
                return Result<T>.Failure($"Download failed: {ex.Message}");
            }
        }

        public async UniTask<Result<T[]>> LoadAll<T>(string[] keys, CancellationToken token = default) where T : DataEntity
        {
            try
            {
                if (keys == null || keys.Length == 0)
                    return Result<T[]>.Failure("DownloadAll - keys array is null or empty.");

                var tasks = keys.Select(key => Load<T>(key, token));
                var results = await UniTask.WhenAll(tasks);

                // Фильтруем успешные результаты
                var successfulResults = results
                    .Where(result => result.IsSuccess)
                    .Select(result => result.Data)
                    .ToArray();

                return Result<T[]>.Success(successfulResults);
            }
            catch (Exception ex)
            {
                return Result<T[]>.Failure($"DownloadAll failed: {ex.Message}");
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
                return Result<T[]>.Failure($"DownloadAllForPrefix failed: {ex.Message}");
            }
        }

        public UniTask<string[]> GetKeysForPrefix(string prefix = null, CancellationToken token = default)
        {
            return UniTask.FromResult(Array.Empty<string>());
        }

        public UniTask<Result> Delete(string key, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }

        public UniTask<Result> DeleteAllForPrefix(string prefix, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            _httpClient.Dispose();
            Debug.Log("RestStorage disposed.");
        }
    }
}