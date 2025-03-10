namespace _Project.System.DataManagementService
{
    public class LocalSync : ISyncStrategy {
        private readonly ILocalStorage _storage;

        public LocalSync(ILocalStorage storage) => _storage = storage;

        public void Execute(SyncJob job) => _storage.Save(job.Key, job.Data);
        public bool Handles(SyncTarget target) => target == SyncTarget.Local;
    }
}