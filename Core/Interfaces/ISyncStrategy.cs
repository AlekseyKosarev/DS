using DS.Core.Enums;
using DS.Core.Sync;

namespace DS.Core.Interfaces
{
    public interface ISyncStrategy {
        void Execute(SyncJob job);
        bool Handles(SyncTarget target); // Добавлено объявление метода
    }
}