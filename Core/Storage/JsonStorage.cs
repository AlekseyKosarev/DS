using System;
using System.IO;
using DS.Core.Interfaces;
using DS.Models;
using Newtonsoft.Json;

namespace DS.Core.Storage
{
    public class JsonStorage : ILocalStorage {
        public void Save(string key, object data) {
            var json = JsonConvert.SerializeObject(data);
            File.WriteAllText(GetPath(key), json);
        }

        public T Load<T>(string key) where T : DataEntity {
            var path = GetPath(key);
            return File.Exists(path) ? JsonConvert.DeserializeObject<T>(File.ReadAllText(path)) : null;
        }

        private string GetPath(string key) => Path.Combine(Environment.CurrentDirectory, $"{key}.json");
    }
}