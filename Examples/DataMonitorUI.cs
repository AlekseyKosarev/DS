using System.Collections;
using System.Threading;
using Cysharp.Threading.Tasks;
using DS.Examples;
using UnityEngine;
using DS.Services;
using TMPro;

public class DataMonitorUI : MonoBehaviour {
    public TextMeshProUGUI cacheText;
    public TextMeshProUGUI localText;
    public TextMeshProUGUI remoteText;

    private DataService _dataService;
    private CancellationTokenSource _cts = new();

    public void Init(DataService dataService) {
        _dataService = dataService;
        StartCoroutine(UpdateDebugInfo());
    }

    private IEnumerator UpdateDebugInfo() {
        while (true) {
            UpdateData().Forget();
            yield return new WaitForSeconds(0.1f); // Обновляем каждую секунду
        }
    }

    private async UniTaskVoid UpdateData() {
        var snapshot = await _dataService.GetDebugSnapshotAsync<ExampleData>("player", _cts.Token);

        // Обновляем UI
        cacheText.text = snapshot.CacheData != null 
            ? $"Cache: {snapshot.CacheData.ToDebugString()}" 
            : "Cache: No data";

        localText.text = snapshot.LocalData.IsSuccess 
            ? $"Local: {snapshot.LocalData.Data.ToDebugString()}" 
            : $"Local: {snapshot.LocalData.ErrorMessage}";

        remoteText.text = snapshot.RemoteData.IsSuccess 
            ? $"Remote: {snapshot.RemoteData.Data.ToDebugString()}" 
            : $"Remote: {snapshot.RemoteData.ErrorMessage}";
    }

    void OnDestroy() {
        _cts.Cancel();
        _cts.Dispose();
    }
}