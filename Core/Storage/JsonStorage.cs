using System;
using System.IO;
using DS.Core.Interfaces;
using DS.Models;
using Newtonsoft.Json;
using UnityEngine;

namespace DS.Core.Storage
{
    public class JsonStorage : ILocalStorage {
        private readonly string _path;

        public JsonStorage(string storagePath) {
            _path = storagePath;
            Directory.CreateDirectory(_path); // Создаем папку при инициализации
        }

        private string GetPath(string key) {
            return Path.Combine(_path, $"{key}.json");
        }

        public void Save(string key, object data) {
            var path = GetPath(key);
            try {
                var json = JsonConvert.SerializeObject(data);
                File.WriteAllText(path, json);
            } catch (Exception ex) {
                Debug.LogError($"Save error: {ex.Message}");
            }
        }

        public T Load<T>(string key) where T : DataEntity {
            var path = GetPath(key);
            if (!File.Exists(path)) return null;
        
            try {
                var json = File.ReadAllText(path);
                return JsonConvert.DeserializeObject<T>(json);
            } catch (Exception ex) {
                Debug.LogError($"Load error: {ex.Message}");
                return null;
            }
        }

        public void Delete(string key) {
            var path = GetPath(key);
            if (File.Exists(path)) {
                File.Delete(path);
            }
        }
    }
}