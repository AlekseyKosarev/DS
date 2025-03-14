using System.Collections;
using System.Linq;
using System.Threading;
using _Project.System.DS.Services;
using _Project.System.DS.Utils;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace _Project.System.DS.Examples
{
    public class DataMonitorUI : MonoBehaviour
    {
        public TextMeshProUGUI cacheText;
        public TextMeshProUGUI localText;
        public TextMeshProUGUI remoteText;
        private readonly CancellationTokenSource _cts = new();

        private DataService _ds;

        private void OnDestroy()
        {
            _cts.Cancel();
            _cts.Dispose();
        }

        public void Init(DataService dataService)
        {
            _ds = dataService;
            StartCoroutine(UpdateDebugInfo());
        }

        private IEnumerator UpdateDebugInfo()
        {
            while (true)
            {
                UpdateDataLayers().Forget();
                yield return new WaitForSeconds(0.1f);
            }
        }

        private async UniTaskVoid UpdateDataLayers()
        {
            var snapshot = await _ds.GetDebugSnapshotAsync<PlayerData>(KeyNamingRules.KeyFor<PlayerData>(), _cts.Token);

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
    }
}