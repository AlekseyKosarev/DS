using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using Cysharp.Threading.Tasks;
using DS.Core.Interfaces;
using DS.Models;
using Newtonsoft.Json;

namespace _Project.System.DS.Core.Storage
{
    public class RestStorage : IRemoteStorage {
        private readonly HttpClient _httpClient;

        public RestStorage(string apiUrl, string authToken) {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(apiUrl);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
        }

        public async UniTask<Result> UploadAsync(string key, object data, CancellationToken token = default) {
            try {
                var json = JsonConvert.SerializeObject(data);
                var content = new StringContent(json, global::System.Text.Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"save/{key}", content, token);
                response.EnsureSuccessStatusCode();
                return Result.Success();
            } catch (Exception ex) {
                return Result.Failure($"Upload failed: {ex.Message}");
            }
        }

        public async UniTask<Result<T>> DownloadAsync<T>(string key, CancellationToken token = default) where T : DataEntity {
            try {
                var response = await _httpClient.GetAsync($"load/{key}", token);
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<T>(json);
                return Result<T>.Success(data);
            } catch (Exception ex) {
                return Result<T>.Failure($"Download failed: {ex.Message}");
            }
        }
    }
}