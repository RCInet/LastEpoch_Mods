using System;
using UnityEngine;

namespace Unity.AssetManager.Core.Editor
{
    [Serializable]
    class TrackedAssetIdentifier : IEquatable<TrackedAssetIdentifier>, IEquatable<AssetIdentifier>
    {
        [SerializeField]
        string m_AssetId;

        [SerializeField]
        string m_ProjectId;

        [SerializeField]
        string m_OrganizationId;

        public string AssetId => m_AssetId ?? string.Empty;
        public string ProjectId => m_ProjectId ?? string.Empty;
        public string OrganizationId => m_OrganizationId ?? string.Empty;

        public TrackedAssetIdentifier() { }

        public TrackedAssetIdentifier(AssetIdentifier identifier)
            : this(identifier.OrganizationId, identifier.ProjectId, identifier.AssetId)
        {
        }

        public TrackedAssetIdentifier(string organizationId, string projectId, string assetId)
        {
            m_AssetId = assetId;
            m_ProjectId = projectId;
            m_OrganizationId = organizationId;
        }

        public virtual bool IsIdValid()
        {
            return !string.IsNullOrEmpty(m_AssetId);
        }

        static bool IsSameId(string str1, string str2)
        {
            return (str1 ?? string.Empty) == (str2 ?? string.Empty);
        }

        public static bool IsFromSameAsset(AssetIdentifier first, AssetIdentifier second)
        {
            return new TrackedAssetIdentifier(first).Equals(new TrackedAssetIdentifier(second));
        }

        public static bool IsFromSameAsset(TrackedAssetIdentifier first, AssetIdentifier second)
        {
            return first.Equals(new TrackedAssetIdentifier(second));
        }

        public virtual bool Equals(TrackedAssetIdentifier other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return IsSameId(m_OrganizationId, other.m_OrganizationId)
                   && IsSameId(m_ProjectId, other.m_ProjectId)
                   && IsSameId(m_AssetId, other.m_AssetId);
        }

        public virtual bool Equals(AssetIdentifier other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            return IsSameId(m_OrganizationId, other.OrganizationId)
                   && IsSameId(m_ProjectId, other.ProjectId)
                   && IsSameId(m_AssetId, other.AssetId);
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

            return obj switch
            {
                AssetIdentifier identifier => Equals(identifier),
                TrackedAssetIdentifier identifier => Equals(identifier),
                _ => false
            };
        }

        public override int GetHashCode()
        {
            var orgIdHash = (OrganizationId ?? string.Empty).GetHashCode();
            var projIdHash = (ProjectId ?? string.Empty).GetHashCode();
            var assetIdHash = (AssetId ?? string.Empty).GetHashCode();

            return HashCode.Combine(orgIdHash, projIdHash, assetIdHash);
        }

        public override string ToString()
        {
            return $"[Org:{OrganizationId}, Proj:{m_ProjectId}, Id:{m_AssetId}]";
        }
    }
}
