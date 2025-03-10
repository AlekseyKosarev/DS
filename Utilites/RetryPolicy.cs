using System;
using System.Threading;

namespace _Project.System.DataManagementService
{
    public class RetryPolicy {
        private readonly int _maxRetries = 5;

        public void Execute(Action action) {
            int attempt = 0;
            while (attempt < _maxRetries) {
                try {
                    action();
                    return;
                } catch {
                    attempt++;
                    Thread.Sleep(TimeSpan.FromSeconds(Math.Pow(2, attempt)));
                }
            }
            throw new Exception("Max retries exceeded");
        }
    }
}