using System;
using System.Threading.Tasks;

namespace Unity.Cloud.CommonEmbedded.Runtime
{
    /// <summary>
    /// An IKeyValueStore implementation that uses browser host capabilities for storage.
    /// </summary>
    class BrowserKeyValueStore : IKeyValueStore
    {
        /// <summary>
        /// Creates a BrowserKeyValueStore instance.
        /// </summary>
        /// <exception cref="PlatformNotSupportedException">Thrown if used outside a Unity WebGL execution context.</exception>
        public BrowserKeyValueStore()
        {
#if !UNITY_WEBGL
            throw new System.PlatformNotSupportedException();
#endif
        }

        /// <inheritdoc />
        public Task DeleteCacheAsync(string filename)
        {
            this.ValidateFilename(filename);
            CommonBrowserInterop.ClearCache(filename);
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task<string> ReadCacheAsync(string filename)
        {
            this.ValidateFilename(filename);
            var result = CommonBrowserInterop.RetrieveCachedValue(filename);
            return Task.FromResult(result);
        }

        /// <inheritdoc />
        public Task WriteToCacheAsync(string filename, string content)
        {
            this.ValidateFilename(filename);
            CommonBrowserInterop.CacheValue(filename, content);
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public Task<bool> ValidateFilenameExistsAsync(string filename)
        {
            this.ValidateFilename(filename);
            return Task.FromResult(!string.IsNullOrEmpty(CommonBrowserInterop.RetrieveCachedValue(filename)));
        }
    }
}
