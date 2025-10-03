using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.AssetManager.Core.Editor
{
    interface IAssetDataAttribute
    {
    }

    [Serializable]
    class AssetDataAttributeCollection
    {
        [SerializeReference]
        List<IAssetDataAttribute> m_Attributes = new ();

        public bool HasAttribute<T>() where T : IAssetDataAttribute
        {
            return HasAttribute(typeof(T));
        }

        public bool HasAttribute(Type type)
        {
            foreach (var attribute in m_Attributes)
            {
                if (attribute.GetType() == type)
                {
                    return true;
                }
            }
            return false;
        }

        public T GetAttribute<T>() where T : class, IAssetDataAttribute
        {
            foreach (var attribute in m_Attributes)
            {
                if (attribute is T typedAttribute)
                {
                    return typedAttribute;
                }
            }
            return null;
        }

        public AssetDataAttributeCollection(params IAssetDataAttribute[] attributes)
        {
            foreach (var attribute in attributes)
            {
                if (HasAttribute(attribute.GetType()))
                {
                    Utilities.DevAssert(false, "Duplicate attribute added to AssetDataAttributes");
                    continue;
                }
                m_Attributes.Add(attribute);
            }
        }

        public AssetDataAttributeCollection(AssetDataAttributeCollection assetDataAttributeCollection)
        {
            if (assetDataAttributeCollection == null)
                return;

            m_Attributes = assetDataAttributeCollection.m_Attributes;
        }

        public AssetDataAttributeCollection()
        {
        }
    }
}
