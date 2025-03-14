using _Project.System.DS.Core.Enums;
using _Project.System.DS.Core.Interfaces;
using _Project.System.DS.Models;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.System.DS.Core.Sync.Strategies
{
    public abstract class BaseSyncStrategy : ISyncStrategy
    {
        public abstract UniTask<Result> SyncIt(SyncJob job);
        public abstract bool Handles(SyncTarget target);

        protected Result CanBeSync(SyncJob job, DataEntity currentData)
        {
            if (IsDataNotExist(currentData)) return Result.Success();
            if (IsVersionOutdated(job, currentData)) return Result.Failure("Data is outdated.");
            //if IsValidated ... etc

            return Result.Success();
        }

        private bool IsDataNotExist(DataEntity currentData)
        {
            return currentData == null;
        }

        private bool IsVersionOutdated(SyncJob job, DataEntity currentData)
        {
            Debug.Log("currentData.Version = " + currentData.Version + " job.Version = " + job.Version);
            return currentData.Version > job.Version;
        }
    }
}