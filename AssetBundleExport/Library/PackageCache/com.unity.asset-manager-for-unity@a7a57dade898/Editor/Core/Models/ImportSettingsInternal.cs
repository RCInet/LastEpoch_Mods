using System;
using UnityEngine;

namespace Unity.AssetManager.Core.Editor
{
    [Serializable]
    struct ImportSettingsInternal
    {
        [SerializeField]
        bool m_SkipImportModal;
        [SerializeField]
        bool m_AvoidRollingBackAssetVersion;
        [SerializeField]
        string m_ImportPath;
        [SerializeField]
        bool m_IsUsingDefaultImportPath;
        [SerializeField]
        ImportOperation.ImportType m_ImportType;

        public bool SkipImportModal => m_SkipImportModal;

        public bool AvoidRollingBackAssetVersion => m_AvoidRollingBackAssetVersion;

        public string ImportPath => m_ImportPath;

        public bool IsUsingDefaultImportPath => m_IsUsingDefaultImportPath;

        public ImportOperation.ImportType ImportType => m_ImportType;

        public ImportSettingsInternal(ImportOperation.ImportType importType, bool skipImportModal, bool avoidRollingBackAssetVersion, string defaultImportPath, string importPath = null)
        {
            m_ImportType = importType;
            m_SkipImportModal = skipImportModal;
            m_AvoidRollingBackAssetVersion = avoidRollingBackAssetVersion;
            m_ImportPath = string.IsNullOrEmpty(importPath) ? defaultImportPath : importPath;
            m_IsUsingDefaultImportPath = string.IsNullOrEmpty(importPath);
        }
    }
}
