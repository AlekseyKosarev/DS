using System;
using System.Collections.Generic;
using DS.Models;

namespace DS.Core.Interfaces
{
    public interface ICacheStorage: IDisposable {
        T Get<T>(string key) where T : DataEntity;
        void Set(string key, DataEntity data, Action onComplete = null, Action<Exception> onError = null);
        
        Result<T[]> GetAll<T>(string[] keys, Action<T[]> onComplete = null, Action<Exception> onError = null) where T : DataEntity;
        string[] GetCacheKeys(string prefix = null);
        void Remove(string key);
        void Clear();
    }
}