using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;

public class AsyncQueue<T> {
    private readonly Queue<T> _queue = new();
    private readonly SemaphoreSlim _semaphore = new(0);
    private CancellationTokenSource _cts = new();

    public void Enqueue(T item) {
        lock (_queue) {
            _queue.Enqueue(item);
            _semaphore.Release();
        }
    }

    public async UniTask<T> DequeueAsync(CancellationToken token = default) {
        while (true) {
            token.ThrowIfCancellationRequested(); // Проверяем токен отмены

            try {
                await _semaphore.WaitAsync(token); // Ждем с токеном отмены
            } catch (OperationCanceledException) {
                throw; // Перебрасываем исключение, если операция отменена
            }

            lock (_queue) {
                if (_queue.Count > 0) {
                    return _queue.Dequeue();
                }
            }
        }
    }

    public void Dispose() {
        _cts?.Cancel();
        _cts?.Dispose();
    }
}