using System;
using Cysharp.Threading.Tasks;
using DS.Core.Enums;
using DS.Core.Interfaces;
using DS.Models;

namespace DS.Core.Sync.Strategies
{
    public class RemoteSync : BaseSyncStrategy
    {
        private readonly IStorage _remoteStorage;

        public RemoteSync(IStorage remoteStorage)
        {
            _remoteStorage = remoteStorage;
        }

        public override bool Handles(SyncTarget target)
        {
            return target == SyncTarget.Remote;
        }

        public override async UniTask<Result>
            SyncIt(SyncJob job) //TODO верояяяятно, тут будет что то особенное...... а пока копипаста
        {
            try
            {
                if (_remoteStorage == null) throw new Exception("RemoteStorage is null");
                if (job == null) throw new Exception("Key is null");

                var currentData = await _remoteStorage.Load<DataEntity>(job.Key);
                var canBeSync = CanBeSync(job, currentData.Data);
                if (canBeSync.IsSuccess)
                {
                    var saveResult = await _remoteStorage.Save(job.Key, job.Data);
                    return saveResult;
                }

                return canBeSync;
            }
            catch (Exception ex)
            {
                return Result.Failure($"RemoteSync failed: {ex.Message}");
            }
        }
    }
}