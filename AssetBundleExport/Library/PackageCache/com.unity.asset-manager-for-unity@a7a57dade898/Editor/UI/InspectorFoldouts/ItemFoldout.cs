using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.AssetManager.Core.Editor;
using UnityEditor;
using UnityEngine.UIElements;

namespace Unity.AssetManager.UI.Editor
{
    abstract class ItemFoldout<TData, TBinding> where TBinding : VisualElement
    {
        readonly Foldout m_Foldout;
        readonly ListView m_ListView;
        protected readonly Toggle m_FoldoutToggle;

        readonly string m_FoldoutExpandedClassName = "details-foldout-expanded";
        readonly string m_FoldoutTitle;

        public bool Expanded
        {
            get => m_Foldout.value;
            set
            {
                m_Foldout.SetValueWithoutNotify(value);
                RefreshFoldoutStyleBasedOnExpansionStatus();
            }
        }

        public bool IsEmpty => m_ListView.itemsSource == null || m_ListView.itemsSource.Count == 0;
        protected IList Items => m_ListView.itemsSource;

        protected abstract TBinding MakeItem();
        protected abstract void BindItem(TBinding element, int index);

        protected event Action<IEnumerable<object>> SelectionChanged;

        protected ItemFoldout(VisualElement parent, string foldoutTitle, string foldoutName, string listViewName,
            string foldoutClassName = null, string listClassName = null, string foldoutExpandedClassName = null)
        {
            m_FoldoutTitle = foldoutTitle;

            m_Foldout = new Foldout
            {
                name = foldoutName,
                viewDataKey = foldoutName,
                text = L10n.Tr(m_FoldoutTitle)
            };
            parent.Add(m_Foldout);

            if (!string.IsNullOrEmpty(foldoutClassName))
                m_Foldout.AddToClassList(foldoutClassName);

            m_ListView = new ListView
            {
                name = listViewName,
                viewDataKey = listViewName,
                selectionType = SelectionType.None,
                focusable = true,
            };
            m_Foldout.Add(m_ListView);
            
            if (!string.IsNullOrEmpty(listClassName))
                m_ListView.AddToClassList(listClassName);

            m_FoldoutToggle = m_Foldout.Q<Toggle>();
            if (m_FoldoutToggle != null)
            {
                m_FoldoutToggle.focusable = false;
            }

            if (!string.IsNullOrEmpty(foldoutExpandedClassName))
            {
                m_FoldoutExpandedClassName = foldoutExpandedClassName;
            }

            m_ListView.selectionChanged += RaiseSelectionChangedEvent;
        }

        public void RegisterValueChangedCallback(Action<bool> action)
        {
            m_Foldout.RegisterValueChangedCallback(_ =>
            {
                RefreshFoldoutStyleBasedOnExpansionStatus();
                action?.Invoke(Expanded);
            });
        }

        public void RefreshFoldoutStyleBasedOnExpansionStatus()
        {
            if (m_Foldout.value)
            {
                m_Foldout.AddToClassList(m_FoldoutExpandedClassName);
            }
            else
            {
                m_Foldout.RemoveFromClassList(m_FoldoutExpandedClassName);
            }
        }

        public void StartPopulating()
        {
            Clear();
            UIElementsUtils.Hide(m_Foldout);
        }

        public void StopPopulating()
        {
            var hasItems = !IsEmpty;
            UIElementsUtils.SetDisplay(m_Foldout, hasItems);
            UIElementsUtils.SetDisplay(m_ListView, hasItems);
        }

        protected virtual IList PrepareListItem(BaseAssetData assetData, IEnumerable<TData> items)
        {
            return items.ToList();
        }

        public virtual void Clear()
        {
            m_Foldout.text = L10n.Tr(m_FoldoutTitle);
            m_ListView.itemsSource = null;
        }

        public virtual void RemoveItems(IEnumerable<TData> items)
        {
            var itemsToRemove = items.ToList();
            var itemsSource = m_ListView.itemsSource as List<TData>;

            if (itemsSource == null)
                return;

            foreach (var item in itemsToRemove)
            {
                itemsSource.Remove(item);
            }

            m_ListView.itemsSource = itemsSource;
            m_Foldout.text = $"{L10n.Tr(m_FoldoutTitle)} ({m_ListView.itemsSource.Count})";
            m_ListView.RefreshItems();
            StopPopulating();
        }

        public void Populate(BaseAssetData assetData, IEnumerable<TData> items)
        {
            m_ListView.itemsSource = PrepareListItem(assetData, items);
            m_ListView.makeItem = MakeItem;
            m_ListView.bindItem = (element, i) => { BindItem((TBinding)element, i); };
            m_ListView.fixedItemHeight = 30;

            m_Foldout.text = $"{L10n.Tr(m_FoldoutTitle)} ({m_ListView.itemsSource.Count})";
        }

        void RaiseSelectionChangedEvent(IEnumerable<object> items)
        {
            SelectionChanged?.Invoke(items);
        }
    }
}
