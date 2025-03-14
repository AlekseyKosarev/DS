using System;
using DS.Models;

namespace DS.Core.Sync
{
    public class SyncJob
    {
        public SyncJob(string key, DataEntity data, int version = 1)
        {
            Key = key;
            Data = data;
            Version = version;
        }

        public string Key { get; }
        public DataEntity Data { get; }
        public int Version { get; set; }
        public DateTime Timestamp { get; } = DateTime.UtcNow;
        public Action OnComplete { get; set; }
        public Action<Exception> OnError { get; set; }
    }
}