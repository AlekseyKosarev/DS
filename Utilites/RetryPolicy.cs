using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace DS.Utilites
{
    public class RetryPolicy {
        private readonly int _maxRetries = 5;

        public async UniTask ExecuteAsync(Func<UniTask> action, CancellationToken token) {
            int attempt = 0;
            while (attempt < _maxRetries) {
                try {
                    await action();
                    return;
                } catch (Exception ex) {
                    attempt++;
                    Debug.LogError($"Retry attempt {attempt} failed: {ex.Message}");
                    var delay = TimeSpan.FromSeconds(Math.Pow(2, attempt));
                    await UniTask.Delay(delay, cancellationToken: token);
                }
            }
            throw new Exception("Max retries exceeded");
        }
    }
}