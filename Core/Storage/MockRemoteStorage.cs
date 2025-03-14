using System.Threading;
using Cysharp.Threading.Tasks;
using DS.Core.Interfaces;
using DS.Models;
using NotImplementedException = System.NotImplementedException;

namespace DS.Core.Storage
{
    public class MockRemoteStorage : IStorage
    {
        public UniTask<Result> Save(string key, DataEntity data, CancellationToken token = default)
        {
            return UniTask.FromResult(Result.Failure("MockRemoteStorage can't save data."));
        }

        public UniTask<Result[]> SaveAll(string[] keys, DataEntity[] data, CancellationToken token = default)
        {
            return UniTask.FromResult(new Result[]{Result.Failure("MockRemoteStorage can't save data.")});
        }

        public UniTask<Result<T>> Load<T>(string key, CancellationToken token = default) where T : DataEntity
        {
            return UniTask.FromResult(Result<T>.Failure("Data not found."));
        }

        public UniTask<Result<T[]>> LoadAll<T>(string[] keys, CancellationToken token = default) where T : DataEntity
        {
            return UniTask.FromResult<Result<T[]>>(Result<T[]>.Failure("Data not found."));
        }

        public UniTask<Result<T[]>> LoadAllForPrefix<T>(string prefix, CancellationToken token = default)
            where T : DataEntity
        {
            return UniTask.FromResult(Result<T[]>.Failure("Data not found."));
        }

        public UniTask<string[]> GetKeysForPrefix(string prefix = null, CancellationToken token = default)
        {
            return new UniTask<string[]>(null);
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
            // Debug.Log("MockRemoteStorage disposed.");
        }
    }
}