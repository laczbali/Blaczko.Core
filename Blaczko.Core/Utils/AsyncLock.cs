namespace Blaczko.Core.Utils
{
    /// <summary>
    /// Use as <br/>
    /// <code>
    /// private AsyncLock asLock = new AsyncLock();
    /// using( await asLock.LockAsync() ) {
    ///     .. things ..
    /// </code>
    /// </summary>
    public class AsyncLock : IDisposable
    {
        private readonly SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1, 1);

        public async Task<AsyncLock> LockAsync()
        {
            await _semaphoreSlim.WaitAsync();
            return this;
        }

        public void Dispose()
        {
            _semaphoreSlim.Release();
        }
    }
}
