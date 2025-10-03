using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Unity.AssetManager.Core.Editor
{
    interface IFileUtility : IService
    {
        bool IsFileDirty(string path);
        Task<ComparisonDetails> FileWasModified(string path, long expectedTimestamp, string expectedChecksum, CancellationToken token);
        long GetTimestamp(string path);
        Task<string> CalculateMD5ChecksumAsync(string path, CancellationToken cancellationToken);
    }

    [Serializable]
    class FileUtility : BaseService<IFileUtility>, IFileUtility
    {
        static readonly int k_MD5_bufferSize = 4096;

        enum ChecksumResult
        {
            Same,
            Different,
            Unknown
        }

        [SerializeReference]
        IEditorUtilityProxy m_EditorUtility;

        [SerializeReference]
        IAssetDatabaseProxy m_AssetDatabase;
        
        [SerializeReference]
        IIOProxy m_IOProxy;

        [ServiceInjection]
        public void Inject(IEditorUtilityProxy editorUtility, IAssetDatabaseProxy assetDatabase, IIOProxy ioProxy)
        {
            m_EditorUtility = editorUtility;
            m_AssetDatabase = assetDatabase;
            m_IOProxy = ioProxy;
        }

        protected override void ValidateServiceDependencies()
        {
            base.ValidateServiceDependencies();

            m_EditorUtility ??= ServicesContainer.instance.Get<IEditorUtilityProxy>();
            m_AssetDatabase ??= ServicesContainer.instance.Get<IAssetDatabaseProxy>();
        }

        public bool IsFileDirty(string path)
        {
            // Check dirty flag
            var asset = m_AssetDatabase.LoadAssetAtPath(path);
            if (asset != null && m_EditorUtility.IsDirty(asset))
            {
                return true;
            }

            // Check if the file is a scene and it is dirty
            if (asset is SceneAsset)
            {
                var scene = SceneManager.GetSceneByPath(path);
                if (scene.isDirty)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<ComparisonDetails> FileWasModified(string path, long expectedTimestamp, string expectedChecksum, CancellationToken token)
        {
            // Locally modified files are always considered dirty
            if (IsFileDirty(path))
            {
                return new ComparisonDetails(ComparisonResults.FilesModified, $"File {Path.GetFileName(path)} is dirty.");
            }

            // Check if the file has the same modified date, in which case we know it wasn't modified
            if (IsSameTimestamp(expectedTimestamp, path))
            {
                return default;
            }

            // Check if we have checksum information, in which case, a similar checksum means the file wasn't modified
            var checksumResult = await IsSameFileChecksumAsync(expectedChecksum, path, token);

            return checksumResult switch
            {
                ChecksumResult.Same => default,

                ChecksumResult.Different => new ComparisonDetails(ComparisonResults.FilesModified, $"File {Path.GetFileName(path)} checksum has changed."),

                // In case we can't determine if the file was modified, we assume it was to avoid blocking the re-upload
                _ => new ComparisonDetails(ComparisonResults.FilesModified, $"Could not determine if file {Path.GetFileName(path)} was modified.")
            };
        }

        public async Task<string> CalculateMD5ChecksumAsync(string path, CancellationToken cancellationToken)
        {
            try
            {
                await using var stream = File.OpenRead(path);
                var checksum = await CalculateMD5ChecksumAsync(stream, cancellationToken);
                return checksum;
            }
            catch (Exception)
            {
                return null;
            }
        }

        bool IsSameTimestamp(long timestamp, string path)
        {
            if (timestamp == 0L)
            {
                return false;
            }

            return timestamp == GetTimestamp(path);
        }

        public long GetTimestamp(string path)
        {
            return ((DateTimeOffset) m_IOProxy.GetFileLastWriteTimeUtc(path)).ToUnixTimeSeconds();
        }

        async Task<ChecksumResult> IsSameFileChecksumAsync(string checksum, string path, CancellationToken token)
        {
            if (string.IsNullOrEmpty(checksum))
            {
                return ChecksumResult.Unknown;
            }

            var localChecksum = await CalculateMD5ChecksumAsync(path, token);
            return checksum == localChecksum ? ChecksumResult.Same : ChecksumResult.Different;
        }

        static async Task<string> CalculateMD5ChecksumAsync(Stream stream, CancellationToken cancellationToken)
        {
            var position = stream.Position;
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

#pragma warning disable S4790 //Using weak hashing algorithms is security-sensitive
                using (var md5 = MD5.Create())
#pragma warning restore S4790
                {
                    var result = new TaskCompletionSource<bool>();
                    await Task.Run(async () =>
                    {
                        try
                        {
                            await CalculateMD5ChecksumAsync(md5, stream, cancellationToken);
                        }
                        finally
                        {
                            result.SetResult(true);
                        }
                    }, cancellationToken);
                    await result.Task;
                    return BitConverter.ToString(md5.Hash).Replace("-", "").ToLowerInvariant();
                }
            }
            finally
            {
                stream.Position = position;
            }
        }

        static async Task CalculateMD5ChecksumAsync(MD5 md5, Stream stream, CancellationToken cancellationToken)
        {
            var buffer = new byte[k_MD5_bufferSize];
            int bytesRead;
            do
            {
                cancellationToken.ThrowIfCancellationRequested();
                bytesRead = await stream.ReadAsync(buffer, 0, k_MD5_bufferSize, cancellationToken);
                if (bytesRead > 0)
                {
                    md5.TransformBlock(buffer, 0, bytesRead, null, 0);
                }
            } while (bytesRead > 0);

            md5.TransformFinalBlock(buffer, 0, 0);
            await Task.CompletedTask;
        }
    }
}
