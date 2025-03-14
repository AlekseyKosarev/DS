using Cysharp.Threading.Tasks;
using DS.Core.Enums;
using DS.Core.Sync;
using DS.Models;

namespace DS.Core.Interfaces
{
    public interface ISyncStrategy
    {
        UniTask<Result> SyncIt(SyncJob job);
        bool Handles(SyncTarget target); // Добавлено объявление метода
    }
}