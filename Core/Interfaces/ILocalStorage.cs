namespace _Project.System.DataManagementService
{
    public interface ILocalStorage {
        void Save(string key, object data);
        T Load<T>(string key) where T : DataEntity;
    }
}