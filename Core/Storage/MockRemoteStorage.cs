using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DS.Core.Interfaces;
using DS.Models;
using UnityEngine;
using NotImplementedException = System.NotImplementedException;

namespace _Project.System.DS.Core.Storage
{
    public class MockRemoteStorage : IRemoteStorage {
        public UniTask<Result> UploadAsync(string key, object data, CancellationToken token = default) {
            // Debug.Log("MockRemoteStorage.Upload = " + key);
            return UniTask.FromResult(Result.Success());
        }

        public UniTask<Result<T>> DownloadAsync<T>(string key, CancellationToken token = default) where T : DataEntity {
            // Debug.Log("MockRemoteStorage.Download = " + key);
            return UniTask.FromResult(Result<T>.Failure("Data not found."));
        }

        public UniTask<Result<T[]>> DownloadAllAsync<T>(string[] keys, CancellationToken token = default) where T : DataEntity
        {
            return UniTask.FromResult(Result<T[]>.Failure("Data not found."));
        }

        public UniTask<string[]> GetRemoteKeysAsync(string prefix = null, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            // Debug.Log("MockRemoteStorage disposed.");
        }
    }
}