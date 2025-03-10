using System.Threading;
using Cysharp.Threading.Tasks;
using DS.Core.Enums;
using DS.Core.Sync;
using DS.Models;

namespace DS.Core.Interfaces
{
    public interface ISyncStrategy {
        UniTask<Result> ExecuteAsync(SyncJob job);
        bool Handles(SyncTarget target); // Добавлено объявление метода
    }
}