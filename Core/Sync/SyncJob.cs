using System;
using _Project.System.DS.Models;

namespace _Project.System.DS.Core.Sync
{
    public class SyncJob {
        public string Key { get; }
        public DataEntity Data { get; }
        public int Version { get; set; }
        public DateTime Timestamp { get; } = DateTime.UtcNow;
        public Action OnComplete { get; set; }
        public Action<Exception> OnError { get; set; }

        public SyncJob(string key, DataEntity data, int version = 1) {
            Key = key;
            Data = data;
            Version = version;
        }
    }
}