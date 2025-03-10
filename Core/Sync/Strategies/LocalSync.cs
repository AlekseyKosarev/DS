using System.Threading;
using Cysharp.Threading.Tasks;
using DS.Core.Enums;
using DS.Core.Interfaces;

namespace DS.Core.Sync.Strategies
{
    public class LocalSync : ISyncStrategy {
        private readonly ILocalStorage _storage;

        public LocalSync(ILocalStorage storage) => _storage = storage;

        public async UniTask ExecuteAsync(SyncJob job, CancellationToken token) => _storage.Save(job.Key, job.Data);
        public bool Handles(SyncTarget target) => target == SyncTarget.Local;
    }
}