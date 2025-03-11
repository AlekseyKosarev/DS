using Cysharp.Threading.Tasks;
using DS.Core.Enums;
using DS.Core.Interfaces;
using DS.Core.Sync;
using DS.Models;

public abstract class BaseSyncStrategy : ISyncStrategy 
{
    protected bool IsJobOutdated(SyncJob job, DataEntity currentData) 
    {
        if (currentData == null) return false; // Нет текущих данных
        if (currentData.Version > job.Version) return true; // Устаревшая версия
        // Дополнительные проверки (например, валидация данных)
        return false;
    }

    public abstract UniTask<Result> ExecuteAsync(SyncJob job);
    public abstract bool Handles(SyncTarget target);
}