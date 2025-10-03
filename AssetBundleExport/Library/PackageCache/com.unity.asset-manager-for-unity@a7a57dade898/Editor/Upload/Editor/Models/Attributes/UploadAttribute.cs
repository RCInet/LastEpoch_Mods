using System;
using Unity.AssetManager.Core.Editor;
using UnityEngine;

namespace Unity.AssetManager.Upload.Editor
{
    [Serializable]
    sealed class UploadAttribute : IAssetDataAttribute, IEquatable<UploadAttribute>
    {
        public enum UploadStatus
        {
            DontUpload,
            Add,
            Skip,
            Override,
            Duplicate,
            ErrorOutsideProject,
            SourceControlled
        }

        [SerializeField]
        UploadStatus m_Status;

        [SerializeField]
        string m_Details;

        public UploadStatus Status => m_Status;
        public string Details => m_Details;

        public UploadAttribute(UploadStatus status, string details)
        {
            m_Status = status;
            m_Details = details;
        }

        public bool Equals(UploadAttribute other)
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

        public static bool operator ==(UploadAttribute left, UploadAttribute right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(UploadAttribute left, UploadAttribute right)
        {
            return !Equals(left, right);
        }
    }
}
