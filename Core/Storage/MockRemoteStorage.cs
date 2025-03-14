using System.Threading;
using _Project.System.DS.Core.Interfaces;
using _Project.System.DS.Models;
using Cysharp.Threading.Tasks;
using NotImplementedException = System.NotImplementedException;

namespace _Project.System.DS.Core.Storage
{
    public class MockRemoteStorage : IStorage
    {
        public UniTask<Result> Save(string key, DataEntity data, CancellationToken token = default)
        {
            // Debug.Log("MockRemoteStorage.Upload = " + key);
            return UniTask.FromResult(Result.Success());
        }

        public UniTask<Result<T>> Load<T>(string key, CancellationToken token = default) where T : DataEntity
        {
            // Debug.Log("MockRemoteStorage.Download = " + key);
            return UniTask.FromResult(Result<T>.Failure("Data not found."));
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