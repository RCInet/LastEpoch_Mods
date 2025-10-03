using System;
using UnityEngine;

namespace Unity.AssetManager.Core.Editor
{
    [Serializable]
    class AssetLabel
    {
        [SerializeField]
        string m_Name;

        [SerializeField]
        string m_Organization;

        public string Name => m_Name;
        public string Organization => m_Organization;

        public AssetLabel(string name, string organization)
        {
            m_Name = name;
            m_Organization = organization;
        }
    }
}
