using System.Threading;
using Cysharp.Threading.Tasks;
using DS.Core.Enums;
using DS.Core.Sync;

namespace DS.Core.Interfaces
{
    public interface ISyncStrategy {
        UniTask ExecuteAsync(SyncJob job, CancellationToken token);
        bool Handles(SyncTarget target); // Добавлено объявление метода
    }
}