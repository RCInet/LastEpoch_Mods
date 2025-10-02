using System;
using UnityEngine;

namespace Unity.AssetManager.Core.Editor
{
    [Serializable]
    class ProjectIdentifier : IEquatable<ProjectIdentifier>
    {
        [SerializeField]
        string m_OrganizationId = string.Empty;

        [SerializeField]
        string m_ProjectId = string.Empty;

        public string OrganizationId => m_OrganizationId ?? string.Empty;
        public string ProjectId => m_ProjectId ?? string.Empty;

        public ProjectIdentifier() { }

        public ProjectIdentifier(string organizationId, string projectId)
        {
            m_OrganizationId = organizationId ?? string.Empty;
            m_ProjectId = projectId ?? string.Empty;
        }

        public override string ToString()
        {
            return $"[Org:{OrganizationId}, Proj:{m_ProjectId}]";
        }

        public bool Equals(ProjectIdentifier other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return m_OrganizationId == other.m_OrganizationId &&
                m_ProjectId == other.m_ProjectId;
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

            return Equals((ProjectIdentifier)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(m_OrganizationId, m_ProjectId);
        }

        public static bool operator ==(ProjectIdentifier left, ProjectIdentifier right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ProjectIdentifier left, ProjectIdentifier right)
        {
            return !Equals(left, right);
        }
    }
}
