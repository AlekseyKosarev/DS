using DS.Models;

namespace DS.Core.Interfaces
{
    public interface ILocalStorage {
        void Save(string key, object data);
        T Load<T>(string key) where T : DataEntity;
        void Delete(string key);
    }
}