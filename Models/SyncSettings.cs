using System;

namespace _Project.System.DS.Models
{
    public class SyncSettings
    {
        public TimeSpan LocalInterval { get; set; } = TimeSpan.FromMinutes(5);
        public TimeSpan RemoteInterval { get; set; } = TimeSpan.FromHours(1);
    }
}