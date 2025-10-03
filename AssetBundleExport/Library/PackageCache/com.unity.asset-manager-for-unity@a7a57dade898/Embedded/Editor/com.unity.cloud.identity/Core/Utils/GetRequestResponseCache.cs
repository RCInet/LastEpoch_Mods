using System;
using System.Collections.Generic;

namespace Unity.Cloud.IdentityEmbedded
{
    internal class GetRequestResponseCache<T>
    {
        // In memory caching of GET requests result based on url value
        readonly Dictionary<string, (DateTime, T)> m_InMemoryCache = new();

        // Time in seconds before the GET request is allowed to reach the service endpoint again.
        readonly int m_TimeLimitInSeconds;

        public GetRequestResponseCache(int timeLimitInSeconds = 10)
        {
            m_TimeLimitInSeconds = timeLimitInSeconds;
        }

        public T AddGetRequestResponseToCache(string url, T value)
        {
            m_InMemoryCache[url] = (DateTime.Now, value);
            return value;
        }

        public bool TryGetRequestResponseFromCache(string url, out T value)
        {
            value = default;
            if (!m_InMemoryCache.ContainsKey(url) ||
                (DateTime.Now - m_InMemoryCache[url].Item1).Seconds >= m_TimeLimitInSeconds) return false;
            value = m_InMemoryCache[url].Item2;
            return true;
        }

    }
}
