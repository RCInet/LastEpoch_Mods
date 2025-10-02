using System;
using System.IO;
using System.Threading.Tasks;

namespace Unity.Cloud.CommonEmbedded
{
    /// <summary>
    /// An interface which abstracts methods related to reading and writing string content to a persistent file storage.
    /// </summary>
    interface IKeyValueStore
    {
        /// <summary>
        /// A Task that reads a cached string from the current <see cref="IKeyValueStore"/>.
        /// </summary>
        /// <param name="filename">The unique filename to look for in the current <see cref="IKeyValueStore"/>.</param>
        /// <returns>
        /// A Task that results in the cached string value.
        /// </returns>
        /// <exception cref="ArgumentException"> Thrown if the filename is null or empty.</exception>
        /// <exception cref="FileNotFoundException"> Thrown if the filename does not exist.</exception>
        Task<string> ReadCacheAsync(string filename);

        /// <summary>
        /// A Task that writes a string content value under a unique filename in the current <see cref="IKeyValueStore"/>.
        /// </summary>
        /// <remarks>
        /// Overwrite any previous value.
        /// </remarks>
        /// <param name="filename">The unique filename that will hold the content value in the current <see cref="IKeyValueStore"/>.</param>
        /// <param name="content">The string content to write.</param>
        /// <returns>A Task that writes a string content value under a unique filename in the current <see cref="IKeyValueStore"/>.</returns>
        /// <exception cref="ArgumentException"> Thrown if the filename is null or empty.</exception>
        Task WriteToCacheAsync(string filename, string content);

        /// <summary>
        /// A Task that deletes a unique filename in the current <see cref="IKeyValueStore"/>.
        /// </summary>
        /// <param name="filename">The unique filename to delete from the current <see cref="IKeyValueStore"/>.</param>
        /// <returns>A Task that deletes a unique filename in the current <see cref="IKeyValueStore"/>.</returns>
        /// <exception cref="ArgumentException"> Thrown if the filename is null or empty.</exception>
        Task DeleteCacheAsync(string filename);

        /// <summary>
        /// A Task that validates if a filename exists in the <see cref="IKeyValueStore"/>.
        /// </summary>
        /// <param name="filename">The unique filename to look for in the current <see cref="IKeyValueStore"/>.</param>
        /// <returns>
        /// A task that has a result of true if the filename is found, false otherwise.
        /// </returns>
        /// <exception cref="ArgumentException">Thrown if the filename is null or empty.</exception>
        Task<bool> ValidateFilenameExistsAsync(string filename);
    }

    /// <summary>
    /// Helper methods for <see cref="IKeyValueStore"/>.
    /// </summary>
    static class IKeyValueStoreExtensions
    {
        /// <summary>
        /// Validates that the filename is not null or empty.
        /// </summary>
        /// <param name="keyValueStore">The KeyValueStore.</param>
        /// <param name="filename">The filename to validate.</param>
        /// <exception cref="ArgumentException">Thrown if the filename is null or empty.</exception>
        internal static void ValidateFilename(this IKeyValueStore keyValueStore, string filename)
        {
            if (string.IsNullOrEmpty(filename))
                throw new ArgumentException("The filename cannot be null or empty.", nameof(filename));
        }
    }
}
