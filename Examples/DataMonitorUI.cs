using System.Collections;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using DS.Core.Utils;
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
        var snapshot = await _dataService.GetDebugSnapshotAsync<PlayerData>("playerdata", _cts.Token);

        // Обновляем UI
        cacheText.text = snapshot.CacheData.IsSuccess 
            ? $"Cache: {string.Join("\n", snapshot.CacheData.Data.Select(x => x.ToDebugString()))}" 
            : $"Cache: {snapshot.CacheData.ErrorMessage}";

        localText.text = snapshot.LocalData.IsSuccess 
            ? $"Local: {string.Join("\n", snapshot.LocalData.Data.Select(x => x.ToDebugString()))}" 
            : $"Local: {snapshot.LocalData.ErrorMessage}";

        remoteText.text = snapshot.RemoteData.IsSuccess 
            ? $"Remote: {string.Join("\n", snapshot.RemoteData.Data.Select(x => x.ToDebugString()))}" 
            : $"Remote: {snapshot.RemoteData.ErrorMessage}";
    }

    void OnDestroy() {
        _cts.Cancel();
        _cts.Dispose();
    }
}