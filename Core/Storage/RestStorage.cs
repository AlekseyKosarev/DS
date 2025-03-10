using System.Net.Http;
using DS.Core.Interfaces;
using DS.Models;
using Newtonsoft.Json;

namespace DS.Core.Storage
{
    public class RestStorage : IRemoteStorage {
        private readonly HttpClient _httpClient = new HttpClient();

        public void Upload(string key, object data) {
            var json = JsonConvert.SerializeObject(data);
            var response = _httpClient.PostAsync($"https://api.example.com/save/{key}", new StringContent(json)).Result;
            response.EnsureSuccessStatusCode();
        }

        public T Download<T>(string key) where T : DataEntity {
            var response = _httpClient.GetStringAsync($"https://api.example.com/load/{key}").Result;
            return JsonConvert.DeserializeObject<T>(response);
        }
    }
}