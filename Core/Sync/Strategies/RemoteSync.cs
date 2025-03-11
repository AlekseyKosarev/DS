using System;
using Cysharp.Threading.Tasks;
using DS.Core.Enums;
using DS.Core.Interfaces;
using DS.Models;

namespace DS.Core.Sync.Strategies
{
    public class RemoteSync : BaseSyncStrategy {
        private readonly IRemoteStorage _remoteStorage;

        public RemoteSync(IRemoteStorage remoteStorage) {
            _remoteStorage = remoteStorage;
        }

        public override bool Handles(SyncTarget target) => target == SyncTarget.Remote;

        public override async UniTask<Result> ExecuteAsync(SyncJob job) {
            try {
                var uploadResult = await _remoteStorage.UploadAsync(job.Key, job.Data);
                return uploadResult;
            } catch (Exception ex) {
                return Result.Failure($"RemoteSync failed: {ex.Message}");
            }
        }
    }
}