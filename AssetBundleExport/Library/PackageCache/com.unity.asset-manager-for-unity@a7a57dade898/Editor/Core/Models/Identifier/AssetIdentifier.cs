using System;
using UnityEngine;

namespace Unity.AssetManager.Core.Editor
{
    [Serializable]
    class AssetIdentifier : IEquatable<AssetIdentifier>
    {
        [SerializeField]
        ProjectIdentifier m_ProjectIdentifier = new();

        [SerializeField]
        string m_AssetId = string.Empty;

        [SerializeField]
        string m_Version = string.Empty;

        [SerializeField]
        string m_VersionLabel = string.Empty;

        public string AssetId => m_AssetId;
        public string Version => m_Version;
        public string VersionLabel
        {
            get => m_VersionLabel;
            internal set => m_VersionLabel = value;
        }

        public string OrganizationId => m_ProjectIdentifier.OrganizationId;
        public string ProjectId => m_ProjectIdentifier.ProjectId;
        public ProjectIdentifier ProjectIdentifier => m_ProjectIdentifier;

        static readonly string k_LocalPrefix = "local";

        public AssetIdentifier()
        {
        }

        public AssetIdentifier(string guid)
            : this(null, null, $"{k_LocalPrefix}_{guid}", "1")
        {
        }

        public AssetIdentifier(string organizationId, string projectId, string assetId, string version)
        {
            m_ProjectIdentifier = new ProjectIdentifier(organizationId, projectId);
            m_AssetId = assetId ?? string.Empty;
            m_Version = version ?? string.Empty;
        }

        public AssetIdentifier(string organizationId, string projectId, string assetId, string version, string versionLabel)
            : this(organizationId, projectId, assetId, version)
        {
            m_VersionLabel = versionLabel ?? string.Empty;
        }

        public override string ToString()
        {
            return $"[Org:{OrganizationId}, Proj:{ProjectId}, Id:{AssetId}, Ver:{Version}]";
        }

        public bool IsLocal()
        {
            return m_AssetId.StartsWith(k_LocalPrefix);
        }

        public AssetIdentifier WithAssetId(string assetId)
        {
            return new AssetIdentifier(m_ProjectIdentifier.OrganizationId, m_ProjectIdentifier.ProjectId, assetId, m_Version);
        }

        public AssetIdentifier WithVersion(string version)
        {
            return new AssetIdentifier(m_ProjectIdentifier.OrganizationId, m_ProjectIdentifier.ProjectId, m_AssetId, version);
        }

        public bool IsIdValid()
        {
            return !string.IsNullOrEmpty(m_AssetId);
        }

        public bool Equals(AssetIdentifier other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Equals(m_ProjectIdentifier, other.m_ProjectIdentifier) &&
                   m_AssetId == other.m_AssetId &&
                   m_Version == other.m_Version;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return Equals((AssetIdentifier)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ProjectIdentifier, AssetId, Version);
        }

        public static bool operator ==(AssetIdentifier left, AssetIdentifier right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(AssetIdentifier left, AssetIdentifier right)
        {
            return !Equals(left, right);
        }
    }
}
