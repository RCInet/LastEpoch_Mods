using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace Unity.AssetManager.Core.Editor
{
    interface ICacheEvictionManager : IService
    {
        void OnCheckEvictConditions(string filePathToAddToCache);
    }

    [Serializable]
    class CacheEvictionManager : BaseService<ICacheEvictionManager>, ICacheEvictionManager
    {
        double m_CurrentSizeMb;

        [SerializeReference]
        IIOProxy m_IOProxy;

        [SerializeReference]
        ISettingsManager m_SettingsManager;

        [ServiceInjection]
        public void Inject(IIOProxy IOProxy, ISettingsManager settingsManager)
        {
            m_IOProxy = IOProxy;
            m_SettingsManager = settingsManager;
        }

        public void OnCheckEvictConditions(string filePathToAddToCache)
        {
            var files = m_IOProxy.GetOldestFilesFromDirectory(m_SettingsManager
                .ThumbnailsCacheLocation);

            if (!files.Any())
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(filePathToAddToCache))
            {
                m_CurrentSizeMb = m_IOProxy.GetFilesSizeMb(files);
                if (m_CurrentSizeMb < m_SettingsManager.MaxCacheSizeMb)
                {
                    return;
                }
            }
            else
            {
                m_CurrentSizeMb += m_IOProxy.GetFileLengthMb(filePathToAddToCache);
                if (m_CurrentSizeMb < m_SettingsManager.MaxCacheSizeMb)
                    return;
            }

            Evict(files, m_CurrentSizeMb);
        }

        private double CalculateSizeToBeRemovedMb(double currentCacheSize)
        {
            if (m_SettingsManager.MaxCacheSizeMb == AssetManagerCoreConstants.DefaultCacheSizeMb)
            {
                return currentCacheSize - (m_SettingsManager.MaxCacheSizeMb - AssetManagerCoreConstants.ShrinkSizeInMb);
            }

            return AssetManagerCoreConstants.ShrinkSizeInMb;
        }

        void Evict(IEnumerable<FileInfo> files, double currentCacheSize)
        {
            var shrinkSize = CalculateSizeToBeRemovedMb(currentCacheSize);

            foreach (var file in files)
            {
                // we received the length in bytes so we transfer in Mb
                shrinkSize -= m_IOProxy.GetFileLengthMb(file);
                m_IOProxy.DeleteFile(file);
                if (shrinkSize <= 0)
                {
                    break;
                }
            }
        }
    }
}
