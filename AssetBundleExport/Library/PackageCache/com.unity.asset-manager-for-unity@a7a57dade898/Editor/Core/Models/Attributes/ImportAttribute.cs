using System;
using UnityEngine;

namespace Unity.AssetManager.Core.Editor
{
    [Serializable]
    sealed class ImportAttribute : IAssetDataAttribute, IEquatable<ImportAttribute>
    {
        public enum ImportStatus
        {
            NoImport,
            UpToDate,
            OutOfDate,
            ErrorSync
        }

        [SerializeField]
        ImportStatus m_Status;

        public ImportStatus Status => m_Status;

        public ImportAttribute(ImportStatus status)
        {
            m_Status = status;
        }

        public bool Equals(ImportAttribute other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return m_Status == other.m_Status;
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
            return HashCode.Combine(Status);
        }

        public static bool operator ==(ImportAttribute left, ImportAttribute right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ImportAttribute left, ImportAttribute right)
        {
            return !Equals(left, right);
        }
    }
}
