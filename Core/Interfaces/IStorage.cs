using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DS.Models;

namespace DS.Core.Interfaces
{
    public interface IStorage : IDisposable
    {
        UniTask<Result> Save(string key, DataEntity data, CancellationToken token = default);
        UniTask<Result[]> SaveAll(string[] keys, DataEntity[] data, CancellationToken token = default);

        UniTask<Result<T>> Load<T>(string key, CancellationToken token = default)
            where T : DataEntity;

        UniTask<Result<T[]>> LoadAll<T>(string[] keys, CancellationToken token = default)
            where T : DataEntity;

        UniTask<Result<T[]>> LoadAllForPrefix<T>(string prefix, CancellationToken token = default)
            where T : DataEntity;

        UniTask<string[]> GetKeysForPrefix(string prefix = null, CancellationToken token = default);

        UniTask<Result> Delete(string key, CancellationToken token = default);
        UniTask<Result> DeleteAllForPrefix(string prefix, CancellationToken token = default);
    }
}