using System;
using Cysharp.Threading.Tasks;
using DS.Core.Enums;
using DS.Core.Interfaces;
using DS.Models;

namespace DS.Core.Sync.Strategies
{
    public class RemoteSync : ISyncStrategy {
        private readonly IRemoteStorage _remoteStorage;

        public RemoteSync(IRemoteStorage remoteStorage) {
            _remoteStorage = remoteStorage;
        }

        public bool Handles(SyncTarget target) => target == SyncTarget.Remote;

        public async UniTask<Result> ExecuteAsync(SyncJob job) {
            try {
                var uploadResult = await _remoteStorage.UploadAsync(job.Key, job.Data);
                return uploadResult;
            } catch (Exception ex) {
                return Result.Failure($"RemoteSync failed: {ex.Message}");
            }
        }
    }
}