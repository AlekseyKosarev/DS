using System;

namespace _Project.System.DataManagementService
{
    public abstract class DataEntity {
        public int Version { get; set; } = 1;
        public DateTime LastModified { get; set; } = DateTime.UtcNow;
    }
}