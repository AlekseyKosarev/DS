using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DS.Core.Enums;
using DS.Core.Interfaces;
using DS.Models;

namespace DS.Core.Sync.Strategies
{
    public class LocalSync : ISyncStrategy {
        private readonly ILocalStorage _localStorage;

        public LocalSync(ILocalStorage localStorage) {
            _localStorage = localStorage;
        }

        public bool Handles(SyncTarget target) => target == SyncTarget.Local;

        public async UniTask<Result> ExecuteAsync(SyncJob job) {
            try {
                var saveResult = await _localStorage.SaveAsync(job.Key, job.Data);
                return saveResult;
            } catch (Exception ex) {
                return Result.Failure($"LocalSync failed: {ex.Message}");
            }
        }
    }
}