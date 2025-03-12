using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DS.Models;

namespace DS.Core.Interfaces
{
    public interface IRemoteStorage: IDisposable  {
        UniTask<Result> UploadAsync(string key, object data, CancellationToken token = default);
        UniTask<Result<T>> DownloadAsync<T>(string key, CancellationToken token = default) where T : DataEntity;
        UniTask<Result<T[]>> DownloadAllAsync<T>(string[] keys, CancellationToken token = default) where T : DataEntity;
        UniTask<string[]> GetRemoteKeysAsync(string prefix = null, CancellationToken token = default);
    }
}