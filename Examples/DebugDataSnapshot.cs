using DS.Models;

public class DebugDataSnapshot<T> where T : DataEntity {
    public Result<T[]> CacheData { get; set; }
    public Result<T[]> LocalData { get; set; }
    public Result<T[]> RemoteData { get; set; }
}