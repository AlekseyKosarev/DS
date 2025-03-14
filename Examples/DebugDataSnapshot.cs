using _Project.System.DS.Models;

namespace _Project.System.DS.Examples
{
    public class DebugDataSnapshot<T> where T : DataEntity
    {
        public Result<T[]> CacheData { get; set; }
        public Result<T[]> LocalData { get; set; }
        public Result<T[]> RemoteData { get; set; }
    }
}