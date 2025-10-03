using System;
using System.IO;
using System.Linq;
using System.Security;
using UnityEngine;

namespace Unity.AssetManager.Core.Editor
{
    interface ICachePathHelper : IService
    {
        void EnsureDirectoryExists(string path);
        CacheLocationValidationResult EnsureBaseCacheLocation(string cacheLocation);
        string CreateAssetManagerCacheLocation(string path);
        string GetDefaultCacheLocation();
    }

    [Serializable]
    class CachePathHelper : BaseService<ICachePathHelper>, ICachePathHelper
    {
        const string k_UnsupportedPlatformError = "Unsupported platform";

        [SerializeReference]
        IApplicationProxy m_ApplicationProxy;

        [SerializeReference]
        IIOProxy m_IOProxy;

        [ServiceInjection]
        public void Inject(IIOProxy ioProxy, IApplicationProxy applicationProxy)
        {
            m_IOProxy = ioProxy;
            m_ApplicationProxy = applicationProxy;
        }

        public string GetDefaultCacheLocation()
        {
            switch (m_ApplicationProxy.Platform)
            {
                case RuntimePlatform.WindowsEditor:
                    return GetWindowsLocation();
                case RuntimePlatform.OSXEditor:
                    return GetMacLocation();
                case RuntimePlatform.LinuxEditor:
                    return GetLinuxLocation();
                default:
                    throw new Exception(k_UnsupportedPlatformError);
            }
        }

        public CacheLocationValidationResult EnsureBaseCacheLocation(string cacheLocation)
        {
            var validationResult = new CacheLocationValidationResult
            {
                Success = false
            };

            try
            {
                EnsureDirectoryExists(cacheLocation);
                var filePath = Path.Combine(m_IOProxy.GetDirectoryInfoFullName(cacheLocation), Path.GetRandomFileName());
                using var fs = m_IOProxy.Create(filePath, 1, FileOptions.DeleteOnClose);

                validationResult.Success = true;
            }
            catch (DirectoryNotFoundException)
            {
                validationResult.ErrorType = CacheValidationResultError.DirectoryNotFound;
            }
            catch (PathTooLongException)
            {
                validationResult.ErrorType = CacheValidationResultError.PathTooLong;
            }
            catch (ArgumentException)
            {
                validationResult.ErrorType = CacheValidationResultError.InvalidPath;
            }
            catch (SecurityException)
            {
                validationResult.ErrorType = CacheValidationResultError.CannotWriteToDirectory;
            }
            catch (Exception)
            {
                validationResult.ErrorType = CacheValidationResultError.CannotWriteToDirectory;
            }

            return validationResult;
        }

        public string CreateAssetManagerCacheLocation(string path)
        {
            string cacheLocation;

            var directory = new DirectoryInfo(path).Name;

            // the base cache location already contains a folder for the asset manager
            if (string.Equals(directory, AssetManagerCoreConstants.AssetManagerCacheLocationFolderName))
            {
                cacheLocation = path;
            }
            else
            {
                cacheLocation = Path.Combine(path, AssetManagerCoreConstants.AssetManagerCacheLocationFolderName);
            }

            if (!m_IOProxy.DirectoryExists(cacheLocation))
            {
                m_IOProxy.CreateDirectory(cacheLocation);
            }

            return cacheLocation;
        }

        public void EnsureDirectoryExists(string path)
        {
            if (!m_IOProxy.DirectoryExists(path))
            {
                m_IOProxy.CreateDirectory(path);
            }
        }

        string GetWindowsLocation()
        {
            return ManageCacheLocation(new[] { "Unity", "cache", AssetManagerCoreConstants.AssetManagerCacheLocationFolderName });
        }

        string GetMacLocation()
        {
            return ManageCacheLocation(new[]
                { "Library", "Unity", "cache", AssetManagerCoreConstants.AssetManagerCacheLocationFolderName });
        }

        string GetLinuxLocation()
        {
            return ManageCacheLocation(new[]
                { ".config", "unity3d", "cache", AssetManagerCoreConstants.AssetManagerCacheLocationFolderName });
        }

        string ManageCacheLocation(string[] paths)
        {
            var location = Path.Combine(Environment.GetFolderPath(
                Environment.SpecialFolder.UserProfile), paths.Aggregate(Path.Combine));

            EnsureDirectoryExists(location);
            return m_IOProxy.GetDirectoryInfoFullName(location);
        }
    }
}
