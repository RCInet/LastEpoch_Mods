using System;
using Unity.AssetManager.Core.Editor;
using UnityEngine;

namespace Unity.AssetManager.UI.Editor
{
    [Serializable]
    class DraggableObjectToImport : ScriptableObject
    {
        [SerializeField]
        AssetIdentifier m_AssetIdentifier;

        public AssetIdentifier AssetIdentifier
        {
            get => m_AssetIdentifier;
            set => m_AssetIdentifier = value;
        }
    }
}
