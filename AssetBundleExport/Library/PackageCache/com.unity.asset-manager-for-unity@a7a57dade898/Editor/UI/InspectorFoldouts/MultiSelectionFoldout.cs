using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.AssetManager.Core.Editor;
using UnityEditor;
using UnityEngine.UIElements;

namespace Unity.AssetManager.UI.Editor
{
    class MultiSelectionFoldout : ItemFoldout<BaseAssetData, MultiSelectionItem>
    {
        static readonly string k_FoldoutClassName = "multi-selection-foldout";
        static readonly string k_CheckMarkName = "unity-checkmark";
        static readonly string k_ViewListName = "view-list";

        List<BaseAssetData> m_FilesList = new();
        readonly Button m_Button;

        public MultiSelectionFoldout(VisualElement parent, string foldoutTitle, string foldoutName, string buttonTitle, Action buttonCallback, string foldoutExpandedClassName = null)
            : base(parent, foldoutTitle, foldoutName, k_ViewListName, k_FoldoutClassName, string.Empty, foldoutExpandedClassName)
        {
            if (m_FoldoutToggle != null)
            {
                var checkmark = m_FoldoutToggle.Q(k_CheckMarkName);
                checkmark.parent.style.flexDirection = FlexDirection.Row;
                var label = m_FoldoutToggle.Q<Label>();
                label.style.position = Position.Relative;
                m_Button = new Button
                {
                    text = L10n.Tr(buttonTitle)
                };
                m_Button.clicked += buttonCallback;
                m_Button.style.position = Position.Relative;
                m_Button.style.paddingLeft = 6;
                m_FoldoutToggle.Add(m_Button);
            }
        }

        public override void Clear()
        {
            base.Clear();
            m_FilesList.Clear();
        }

        public override void RemoveItems(IEnumerable<BaseAssetData> items)
        {
            var list = items.ToList();
            base.RemoveItems(list);

            foreach (var item in list)
            {
                m_FilesList.Remove(item);
            }
        }

        public void SetButtonEnable(bool enabled)
        {
            m_Button.SetEnabled(enabled);
        }

        protected override IList PrepareListItem(BaseAssetData assetData, IEnumerable<BaseAssetData> items)
        {
            m_FilesList = new List<BaseAssetData>();

            foreach (var assetDataFile in items.OrderBy(f => f.Name))
            {
                m_FilesList.Add(assetDataFile);
            }

            return m_FilesList;
        }

        protected override MultiSelectionItem MakeItem()
        {
            return new MultiSelectionItem();
        }

        protected override void BindItem(MultiSelectionItem element, int index)
        {
            var fileItem = m_FilesList[index];
            TaskUtils.TrackException(element.Refresh(fileItem));
        }
    }
}
