using System;

namespace _Project.System.DataManagementService
{
    public interface ICacheStorage {
        T Get<T>(string key) where T : DataEntity;
        void Set(string key, DataEntity data, TimeSpan ttl, Action onComplete = null, Action<Exception> onError = null);
        void Remove(string key);
    }
}