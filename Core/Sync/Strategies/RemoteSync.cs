using DS.Core.Enums;
using DS.Core.Interfaces;
using DS.Utilites;

namespace DS.Core.Sync.Strategies
{
    public class RemoteSync : ISyncStrategy {
        private readonly IRemoteStorage _remote;
        private readonly RetryPolicy _retryPolicy;

        public RemoteSync(IRemoteStorage remote, RetryPolicy retryPolicy) {
            _remote = remote;
            _retryPolicy = retryPolicy;
        }

        public void Execute(SyncJob job) => _retryPolicy.Execute(() => _remote.Upload(job.Key, job.Data));
        public bool Handles(SyncTarget target) => target == SyncTarget.Remote;
    }
}