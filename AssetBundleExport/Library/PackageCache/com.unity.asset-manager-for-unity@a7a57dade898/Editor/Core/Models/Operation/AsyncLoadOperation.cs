using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Unity.AssetManager.Core.Editor
{
    // Nothing in this class actually needs to be serialized, but if we don't serialize it, we will encounter null exception errors when we update code
    // Consider cancelling the operation on domain reload
    [Serializable]
    class AsyncLoadOperation
    {
        Action m_CancelledCallback;

        CancellationTokenSource m_TokenSource;
        bool m_IsLoading;

        public bool IsLoading => m_IsLoading;

        public Task Start<T>(Func<CancellationToken, IAsyncEnumerable<T>> createTaskFromToken,
            Action loadingStartCallback = null,
            Action<IEnumerable<T>> successCallback = null,
            Action<T> onItemCallback = null,
            Action cancelledCallback = null,
            Action<Exception> exceptionCallback = null,
            Action finallyCallback = null)
        {
            if (IsLoading || createTaskFromToken == null)
                return Task.CompletedTask;

            var token = CreateToken();

            var asyncEnumerableTask = CreateAsyncEnumerableTask(createTaskFromToken, onItemCallback, token);
            return Start(asyncEnumerableTask, loadingStartCallback, successCallback,
                cancelledCallback, exceptionCallback, finallyCallback, token);
        }

        public Task Start<T>(Func<CancellationToken, Task<T>> createTaskFromToken,
            Action loadingStartCallback = null,
            Action<T> successCallback = null,
            Action cancelledCallback = null,
            Action<Exception> exceptionCallback = null,
            Action finallyCallback = null)
        {
            if (IsLoading || createTaskFromToken == null)
                return Task.CompletedTask;

            var token = CreateToken();

            return Start(createTaskFromToken(token), loadingStartCallback, successCallback,
                cancelledCallback, exceptionCallback, finallyCallback, token);
        }

        async Task Start<T>(Task<T> task,
            Action loadingStartCallback,
            Action<T> successCallback,
            Action cancelledCallback,
            Action<Exception> exceptionCallback,
            Action finallyCallback,
            CancellationToken token)
        {
            m_CancelledCallback = cancelledCallback;

            m_IsLoading = true;
            loadingStartCallback?.Invoke();

            try
            {
                var result = await task;

                token.ThrowIfCancellationRequested();

                m_IsLoading = false;
                successCallback?.Invoke(result);
            }
            catch (OperationCanceledException)
            {
                // We do nothing here because when `CancelLoading` is called,
                // `onLoadingCancelled` is also triggered already, the only thing left is to dispose the token source
            }
            catch (Exception e)
            {
                exceptionCallback?.Invoke(e);
            }
            finally
            {
                m_IsLoading = false;
                DisposeToken(token);
                finallyCallback?.Invoke();
            }
        }

        public void Cancel()
        {
            if (m_TokenSource == null)
                return;

            m_TokenSource.Cancel();

            m_CancelledCallback?.Invoke();
            m_CancelledCallback = null;
        }

        CancellationToken CreateToken()
        {
            m_TokenSource = new CancellationTokenSource();
            return m_TokenSource.Token;
        }

        void DisposeToken(CancellationToken token)
        {
            // Only dispose the token source if it's the same token
            // This is to prevent disposing the token source when the operation is cancelled and a new token source is created
            if (m_TokenSource != null && m_TokenSource.Token == token)
            {
                m_TokenSource.Dispose();
                m_TokenSource = null;
            }
        }

        static async Task<List<T>> CreateAsyncEnumerableTask<T>(Func<CancellationToken, IAsyncEnumerable<T>> createTaskFromToken,
            Action<T> onItemCallback, CancellationToken token)
        {
            var result = new List<T>();

            await foreach (var item in createTaskFromToken(token))
            {
                onItemCallback?.Invoke(item);
                result.Add(item);
            }

            return result;
        }
    }
}
