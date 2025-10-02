using System;
using System.Collections.Generic;
using System.Linq;
using Unity.AssetManager.Core.Editor;
using UnityEditor;
using UnityEngine;

namespace Unity.AssetManager.UI.Editor
{
    interface IStateManager : IService
    {
        float SideBarScrollValue { get; set; }
        ISet<string> UncollapsedCollections { get; }
        float SideBarWidth { get; set; }
        bool DependenciesFoldoutValue { get; set; }
        bool[] MultiSelectionFoldoutsValues { get; }

        bool GetFilesFoldoutValue(string key);
        void SetFilesFoldoutValue(string key, bool value);
    }

    [Serializable]
    class StateManager : BaseService<IStateManager>, IStateManager, ISerializationCallbackReceiver
    {
        [Serializable]
        struct FileFoldoutState
        {
            public string Key;
            public bool Value;
        }

        [SerializeField]
        string[] m_SerializedUncollapsedCollections = Array.Empty<string>();

        [SerializeField]
        float m_SideBarScrollValue;

        [SerializeField]
        FileFoldoutState[] m_SerializedFilesFoldoutValues;

        [SerializeField]
        bool m_DependenciesFoldoutValue;

        [SerializeField]
        bool[] m_MultiSelectionFoldoutsValues = new bool[Enum.GetValues(typeof(MultiAssetDetailsPage.FoldoutName)).Cast<MultiAssetDetailsPage.FoldoutName>().Distinct().Count()];

        Dictionary<string, bool> m_FilesFoldoutValues = new();
        
        static readonly string k_SideBarWidthPrefKey = "com.unity.asset-manager-for-unity.side-bar-width";

        HashSet<string> m_UncollapsedCollections = new();

        public float SideBarScrollValue
        {
            get => m_SideBarScrollValue;
            set
            {
                if (float.IsNaN(value) || float.IsInfinity(value))
                    return;

                m_SideBarScrollValue = value;
            }
        }

        public ISet<string> UncollapsedCollections => m_UncollapsedCollections;

        public float SideBarWidth
        {
            get => EditorPrefs.GetFloat(k_SideBarWidthPrefKey, 160.0f);
            set
            {
                if (float.IsNaN(value) || float.IsInfinity(value))
                    return;

                EditorPrefs.SetFloat(k_SideBarWidthPrefKey, value);
            }
        }

        public bool DependenciesFoldoutValue
        {
            get => m_DependenciesFoldoutValue;
            set => m_DependenciesFoldoutValue = value;
        }

        public bool[] MultiSelectionFoldoutsValues
        {
            get => m_MultiSelectionFoldoutsValues;
            set => m_MultiSelectionFoldoutsValues = value;
        }
        
        public bool GetFilesFoldoutValue(string key)
        {
            return m_FilesFoldoutValues.GetValueOrDefault(key, false);
        }
        
        public void SetFilesFoldoutValue(string key, bool value)
        {
            m_FilesFoldoutValues[key] = value;
        }

        public void OnBeforeSerialize()
        {
            m_SerializedUncollapsedCollections = UncollapsedCollections.ToArray();
            
            m_SerializedFilesFoldoutValues = m_FilesFoldoutValues.Select(kv => new FileFoldoutState { Key = kv.Key, Value = kv.Value }).ToArray();
        }

        public void OnAfterDeserialize()
        {
            m_UncollapsedCollections = new HashSet<string>(m_SerializedUncollapsedCollections);

            m_FilesFoldoutValues = m_SerializedFilesFoldoutValues?.ToDictionary(kv => kv.Key, kv => kv.Value) ?? new Dictionary<string, bool>();
        }
    }
}
