using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DS.Models;

namespace DS.Core.Interfaces
{
    public interface ILocalStorage: IDisposable  {
        UniTask<Result> SaveAsync(string key, object data, CancellationToken token = default);
        UniTask<Result<T>> LoadAsync<T>(string key, CancellationToken token = default) where T : DataEntity;
        UniTask<Result> DeleteAsync(string key, CancellationToken token = default);
    }
}