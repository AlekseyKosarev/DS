using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DS.Models;

namespace DS.Core.Interfaces
{
    public interface ILocalStorage: IDisposable  {
        UniTask<Result> SaveAsync(string key, object data, CancellationToken token = default);
        UniTask<Result<T>> LoadAsync<T>(string key, CancellationToken token = default) where T : DataEntity;
        UniTask<Result<T[]>> LoadAllAsync<T>(string[] keys, CancellationToken token = default) where T : DataEntity;
        UniTask<string[]> GetLocalKeysAsync(string prefix = null, CancellationToken token = default);

        UniTask<Result> DeleteAsync(string key, CancellationToken token = default);
    }
}