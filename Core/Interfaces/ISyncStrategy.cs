namespace _Project.System.DataManagementService
{
    public interface ISyncStrategy {
        void Execute(SyncJob job);
        bool Handles(SyncTarget target); // Добавлено объявление метода
    }
}