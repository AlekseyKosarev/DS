using System.Threading;
using Cysharp.Threading.Tasks;
using DS.Models;

namespace DS.Core.Interfaces
{
    public interface IRemoteStorage {
        UniTask<Result> UploadAsync(string key, object data, CancellationToken token = default);
        UniTask<Result<T>> DownloadAsync<T>(string key, CancellationToken token = default) where T : DataEntity;
    }
}