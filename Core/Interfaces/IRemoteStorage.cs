using DS.Models;

namespace DS.Core.Interfaces
{
    public interface IRemoteStorage {
        void Upload(string key, object data);
        T Download<T>(string key) where T : DataEntity;
    }
}