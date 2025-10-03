using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.IdentityEmbedded
{
    internal class PageRequest<T>
    {
        readonly Func<string, CancellationToken, Task<AssetProjectPageResultsJson<T>>> m_PageFunction;

        readonly int m_MaxResultPerRequest;

        public PageRequest(Func<string, CancellationToken, Task<AssetProjectPageResultsJson<T>>> pageFunction, int maxResultPerRequest)
        {
            m_PageFunction = pageFunction;
            m_MaxResultPerRequest = maxResultPerRequest;
        }

        public async IAsyncEnumerable<T> Execute(string requestBasePath, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            int totalRequestResults;
            var currentPage = 1;
            do
            {
                cancellationToken.ThrowIfCancellationRequested();
                var pageRequestPath = $"{requestBasePath}?page={currentPage}&limit={m_MaxResultPerRequest}";
                var pageResultsJson = await m_PageFunction(pageRequestPath, cancellationToken);
                totalRequestResults = pageResultsJson.Projects.Count();
                foreach (var resultJson in pageResultsJson.Projects)
                {
                    yield return resultJson;
                }
                // Increment page for next request
                currentPage++;
            }
            // while there are still results to come
            while (totalRequestResults == m_MaxResultPerRequest);
        }
    }
}
