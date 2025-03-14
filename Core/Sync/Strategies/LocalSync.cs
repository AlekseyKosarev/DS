using System;
using _Project.System.DS.Core.Enums;
using _Project.System.DS.Core.Interfaces;
using _Project.System.DS.Models;
using Cysharp.Threading.Tasks;

namespace _Project.System.DS.Core.Sync.Strategies
{
    public class LocalSync : BaseSyncStrategy
    {
        private readonly IStorage _localStorage;

        public LocalSync(IStorage localStorage)
        {
            _localStorage = localStorage;
        }

        public override bool Handles(SyncTarget target)
        {
            return target == SyncTarget.Local;
        }

        public override async UniTask<Result> SyncIt(SyncJob job)
        {
            try
            {
                if (_localStorage == null) throw new Exception("RemoteStorage is null");
                if (job == null) throw new Exception("Key is null");

                var currentData = await _localStorage.Load<DataEntity>(job.Key);
                var canBeSync = CanBeSync(job, currentData.Data);
                if (canBeSync.IsSuccess)
                {
                    var saveResult = await _localStorage.Save(job.Key, job.Data);
                    return saveResult;
                }

                return canBeSync;
            }
            catch (Exception ex)
            {
                return Result.Failure($"LocalSync failed: {ex.Message}");
            }
        }
    }
}