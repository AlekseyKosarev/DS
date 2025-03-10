using System;

namespace _Project.System.DataManagementService
{
    public class SyncJob {
        public string Key { get; }
        public object Data { get; }
        public int Version { get; set; }
        public DateTime Timestamp { get; } = DateTime.UtcNow;
        public Action OnComplete { get; set; }
        public Action<Exception> OnError { get; set; }

        public SyncJob(string key, object data, int version = 1) {
            Key = key;
            Data = data;
            Version = version;
        }
    }
}