using System;
using System.Collections.Generic;
using System.Linq;
using Unity.AssetManager.Core.Editor;

namespace Unity.AssetManager.UI.Editor
{
    class AssetDataSelection
    {
        public Action<BaseAssetData, AssetDataEventType> AssetDataChanged;

        List<BaseAssetData> m_Selection = new();

        public IReadOnlyCollection<BaseAssetData> Selection
        {
            get => m_Selection;
            set
            {
                Clear();

                m_Selection = value.ToList();

                foreach (var assetData in m_Selection)
                {
                    assetData.AssetDataChanged += OnAssetDataEvent;
                }
            }
        }

        void OnAssetDataEvent(BaseAssetData assetData, AssetDataEventType eventType)
        {
            AssetDataChanged?.Invoke(assetData, eventType);
        }

        public bool Exists(Func<BaseAssetData, bool> func)
        {
            return m_Selection.Exists(x => func(x));
        }

        public void Clear()
        {
            foreach (var assetData in m_Selection)
            {
                assetData.AssetDataChanged -= OnAssetDataEvent;
            }
        }
    }
}
