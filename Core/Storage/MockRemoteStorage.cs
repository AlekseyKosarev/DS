using System.Threading;
using Cysharp.Threading.Tasks;
using DS.Core.Interfaces;
using DS.Models;
using UnityEngine;

namespace _Project.System.DS.Core.Storage
{
    public class MockRemoteStorage : IRemoteStorage {
        public UniTask<Result> UploadAsync(string key, object data, CancellationToken token = default) {
            Debug.Log("MockRemoteStorage.Upload = " + key);
            return UniTask.FromResult(Result.Success());
        }

        public UniTask<Result<T>> DownloadAsync<T>(string key, CancellationToken token = default) where T : DataEntity {
            Debug.Log("MockRemoteStorage.Download = " + key);
            return UniTask.FromResult(Result<T>.Failure("Data not found."));
        }
    }
}