using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.AssetManager.UI.Editor
{
    class SearchFilterChip : VisualElement
    {
        const string k_UssName = "SearchFilterChip";
        const string k_CancelButtonUnityUssStyleClass = "unity-search-field-base__cancel-button";
        const string k_SearchChipIMGUIStyleName = "CN CountBadge";
        const int k_ChipSizeDelta = -1;

        IMGUIContainer m_SearchChipContainer;

        Action<string> m_Dismiss;

        internal string SearchFilter { get; private set; }

#pragma warning disable CS0618 // Type or member is obsolete
        public new class UxmlFactory : UxmlFactory<SearchFilterChip> { }
#pragma warning restore CS0618 // Type or member is obsolete

        public SearchFilterChip() { }

        internal SearchFilterChip(string searchFilter, Action<string> dismissCallback)
        {
            Initialize(searchFilter, dismissCallback);
        }

        void Initialize(string searchFilter, Action<string> dismissCallback)
        {
            AddToClassList(k_UssName);

            SearchFilter = searchFilter;
            m_Dismiss = dismissCallback;

            style.flexDirection = FlexDirection.Row;
            style.height = EditorGUIUtility.singleLineHeight + k_ChipSizeDelta;

            m_SearchChipContainer = new IMGUIContainer(OnGUIHandler);
            m_SearchChipContainer.style.color = new StyleColor(Color.blue);
            m_SearchChipContainer.style.flexDirection = FlexDirection.Row;

            var searchFilterLabel = new Label(SearchFilter);
            var clearButton = new Button(() => dismissCallback.Invoke(SearchFilter));
            clearButton.AddToClassList(k_CancelButtonUnityUssStyleClass);

            Add(searchFilterLabel);
            Add(clearButton);
        }

        ~SearchFilterChip()
        {
            m_SearchChipContainer.Dispose();
        }

        internal void DismissSearchChip()
        {
            m_Dismiss?.Invoke(SearchFilter);
        }

        void OnGUIHandler()
        {
            GUILayout.Box("", k_SearchChipIMGUIStyleName);
        }
    }
}
