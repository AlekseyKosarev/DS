using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DS.Core.Enums;
using DS.Core.Interfaces;
using DS.Models;

namespace DS.Core.Sync.Strategies
{
    public class LocalSync : BaseSyncStrategy {
        private readonly ILocalStorage _localStorage;

        public LocalSync(ILocalStorage localStorage) {
            _localStorage = localStorage;
        }

        public override bool Handles(SyncTarget target) => target == SyncTarget.Local;

        public override async UniTask<Result> ExecuteAsync(SyncJob job) {
            try {
                var currentData = await _localStorage.LoadAsync<DataEntity>(job.Key);
                if (IsJobOutdated(job, currentData.Data)) {
                    return Result.Success(); // Данные уже актуальны
                }
                
                // Данные устарели
                var saveResult = await _localStorage.SaveAsync(job.Key, job.Data);
                return saveResult;
            } catch (Exception ex) {
                return Result.Failure($"LocalSync failed: {ex.Message}");
            }
        }
    }
}