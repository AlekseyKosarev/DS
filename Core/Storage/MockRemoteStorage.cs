using DS.Core.Interfaces;
using DS.Models;
using UnityEngine;

public class MockRemoteStorage: IRemoteStorage
{
    public void Upload(string key, object data)
    {
        Debug.Log("MockRemoteStorage.Upload = " + key);
    }

    public T Download<T>(string key) where T : DataEntity
    {
        Debug.Log("MockRemoteStorage.Download = " + key);
        return null;
    }
}