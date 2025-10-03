using System;
using System.IO;
using System.Threading.Tasks;

namespace Unity.Cloud.CommonEmbedded
{
    /// <summary>
    /// An IKeyValueStore implementation that uses text files for storage.
    /// </summary>
    class FileKeyValueStore : IKeyValueStore
    {
        readonly string m_CacheFilePath;
        readonly IStringObfuscator m_StringObfuscator;

        /// <summary>
        /// Creates a FileKeyValueStore instance.
        /// </summary>
        /// <param name="cacheFilePath">
        /// The platform-specific base path for all stored data.
        /// </param>
        /// <param name="stringObfuscator">
        /// The optional IStringObfuscator implementation.
        /// </param>
        public FileKeyValueStore(string cacheFilePath, IStringObfuscator stringObfuscator = null)
        {
            m_CacheFilePath = cacheFilePath;
            m_StringObfuscator = stringObfuscator;
        }

        /// <inheritdoc />
        public async Task<string> ReadCacheAsync(string filename)
        {
            this.ValidateFilename(filename);

            var filepath = Path.Combine(m_CacheFilePath, filename);
            if (!File.Exists(filepath))
                throw new FileNotFoundException("The file does not exist.", filename);

            return await ReadFileContents(filepath);
        }

        /// <inheritdoc />
        public async Task WriteToCacheAsync(string filename, string content)
        {
            this.ValidateFilename(filename);

            await DeleteCacheAsync(filename).ConfigureAwait(false);

            var filepath = Path.Combine(m_CacheFilePath, filename);

            if (!string.IsNullOrEmpty(content))
                content = m_StringObfuscator?.Encrypt(content) ?? content;

            if (content != null)
                await File.WriteAllTextAsync(filepath, content).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task DeleteCacheAsync(string filename)
        {
            this.ValidateFilename(filename);

            var filepath = Path.Combine(m_CacheFilePath, filename);
            if (File.Exists(filepath))
                File.Delete(filepath);

            await Task.CompletedTask.ConfigureAwait(false);
        }

        /// <inheritdoc />
        public Task<bool> ValidateFilenameExistsAsync(string filename)
        {
            this.ValidateFilename(filename);
            return Task.FromResult(File.Exists(filename));
        }

        async Task<string> ReadFileContents(string filepath)
        {
            var fileContent = await File.ReadAllTextAsync(filepath).ConfigureAwait(false);

            if (string.IsNullOrEmpty(fileContent))
                return string.Empty;

            try
            {
                return m_StringObfuscator != null ? m_StringObfuscator.Decrypt(fileContent) : fileContent;
            }
            catch (ArgumentNullException e)
            {
                throw new ArgumentException(e.Message);
            }
        }
    }
}
