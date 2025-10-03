using System;
using Unity.AssetManager.Core.Editor;
using UnityEngine;

namespace Unity.AssetManager.Upload.Editor
{
    [Serializable]
    sealed class LinkedDependencyAttribute : IAssetDataAttribute, IEquatable<LinkedDependencyAttribute>
    {
        [SerializeField]
        bool m_IsLinked;

        public bool IsLinked => m_IsLinked;

        public LinkedDependencyAttribute(bool linked)
        {
            m_IsLinked = linked;
        }

        public bool Equals(LinkedDependencyAttribute other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return m_IsLinked == other.m_IsLinked;
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
            return HashCode.Combine(IsLinked);
        }

        public static bool operator ==(LinkedDependencyAttribute left, LinkedDependencyAttribute right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(LinkedDependencyAttribute left, LinkedDependencyAttribute right)
        {
            return !Equals(left, right);
        }
    }
}
