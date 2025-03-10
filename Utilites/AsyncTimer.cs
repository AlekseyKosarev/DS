// AsyncTimer.cs (в папке Utilities)
using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class AsyncTimer : IDisposable {
    private CancellationTokenSource _cts;
    private bool _isRunning;

    public void Start(TimeSpan interval, Func<CancellationToken, UniTask> action) {
        _cts = new CancellationTokenSource();
        _isRunning = true;
        RunTimer(interval, action, _cts.Token).Forget();
    }

    private async UniTaskVoid RunTimer(TimeSpan interval, Func<CancellationToken, UniTask> action, CancellationToken token) {
        while (!_cts.IsCancellationRequested) {
            try {
                await action(token);
                await UniTask.Delay(interval, cancellationToken: token);
            } catch (OperationCanceledException) {
                break;
            } catch (Exception ex) {
                Debug.LogError($"Timer error: {ex.Message}");
                break;
            }
        }
        _isRunning = false;
    }

    public void Stop() {
        _cts?.Cancel();
        _cts?.Dispose();
        _cts = null;
    }

    public void Dispose() {
        Stop();
    }

    public bool IsRunning => _isRunning;
}