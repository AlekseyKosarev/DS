using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class AsyncQueue<T> : IDisposable {
    private readonly Queue<T> _queue = new();
    private readonly SemaphoreSlim _semaphore = new(0);
    private bool _disposed;

    public void Enqueue(T item) {
        lock (_queue) {
            _queue.Enqueue(item);
            _semaphore.Release();
        }
    }

    public async UniTask<T> DequeueAsync(CancellationToken token = default) {
        while (!_disposed) {
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

        throw new ObjectDisposedException(nameof(AsyncQueue<T>));
    }

    public void Dispose() {
        _disposed = true;
        _semaphore.Dispose();
        // Debug.Log("AsyncQueue disposed.");
    }
}