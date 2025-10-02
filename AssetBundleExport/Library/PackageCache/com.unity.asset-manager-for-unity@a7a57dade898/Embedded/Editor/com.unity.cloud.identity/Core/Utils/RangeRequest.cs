using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.IdentityEmbedded
{
    internal class RangeRequest<T>
    {
        readonly Func<string, CancellationToken, Task<RangeResultsJson<T>>> m_RangeFunction;

        string m_RequestBasePath;
        readonly int m_MaxResultPerRequest;

        public RangeRequest(Func<string, CancellationToken, Task<RangeResultsJson<T>>> rangeFunction, int maxResultPerRequest)
        {
            m_RangeFunction = rangeFunction;
            m_MaxResultPerRequest = maxResultPerRequest;
        }

        public async IAsyncEnumerable<T> Execute(string requestBasePath, Range range, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            m_RequestBasePath = requestBasePath;
            cancellationToken.ThrowIfCancellationRequested();
            var initialOffsetAndLength = await GetOffsetAndLengthAsync(range, cancellationToken);

            if (initialOffsetAndLength.Length <= 0)
                yield break;

            var subRange = (initialOffsetAndLength.Offset, initialOffsetAndLength.Length, initialOffsetAndLength.Max);
            do
            {
                cancellationToken.ThrowIfCancellationRequested();
                var rangeRequestPath = $"{m_RequestBasePath}?offset={subRange.Offset}&limit={subRange.Length}";
                var rangeResultsJson = await m_RangeFunction(rangeRequestPath, cancellationToken);
                foreach (var resultJson in rangeResultsJson.Results)
                {
                    yield return resultJson;
                }
                // Increment offset for next request
                subRange = (subRange.Offset + subRange.Length, subRange.Length, subRange.Max);
            }
            // while offset is not larger than max
            while (subRange.Offset < subRange.Max);
        }

        async Task<(int Offset, int Length, int Max)> GetOffsetAndLengthAsync(Range range, CancellationToken cancellationToken)
        {
            var count = await GetSourceCountAsync(cancellationToken);
            (int Offset, int Length, int Max) values = (0, 0, count);

            var normalizedRange = range.NormalizeRange(count);
            var rangeCount = normalizedRange.End.Value - normalizedRange.Start.Value;

            values.Offset = normalizedRange.Start.Value;
            values.Length = Math.Min(rangeCount, m_MaxResultPerRequest);
            values.Max = normalizedRange.End.Value;
            return values;
        }

        async Task<int> GetSourceCountAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var rangeRequestPath = $"{m_RequestBasePath}?offset=0&limit=1";
            return (await m_RangeFunction(rangeRequestPath, cancellationToken)).Total;
        }
    }
}
