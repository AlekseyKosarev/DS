using _Project.System.DS.Core.Enums;
using _Project.System.DS.Core.Sync;
using _Project.System.DS.Models;
using Cysharp.Threading.Tasks;

namespace _Project.System.DS.Core.Interfaces
{
    public interface ISyncStrategy {
        UniTask<Result> SyncIt(SyncJob job);
        bool Handles(SyncTarget target); // Добавлено объявление метода
    }
}