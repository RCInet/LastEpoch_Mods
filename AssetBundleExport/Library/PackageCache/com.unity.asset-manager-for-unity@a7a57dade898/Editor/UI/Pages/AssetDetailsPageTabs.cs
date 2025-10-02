using System;
using System.Collections.Generic;
using Unity.AssetManager.Core.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.AssetManager.UI.Editor
{
    class AssetDetailsPageTabs : IPageComponent
    {
        public enum TabType
        {
            Details,
            Versions
        }

        struct TabDetails
        {
            public Button TabButton { get; }
            public VisualElement TabContent { get; }
            public bool DisplayFooter { get; }
            public bool EnabledWhenDisconnected { get; }

            public TabDetails(Button tabButton, VisualElement tabContent, bool displayFooter, bool enabledWhenDisconnected)
            {
                TabButton = tabButton;
                TabContent = tabContent;
                DisplayFooter = displayFooter;
                EnabledWhenDisconnected = enabledWhenDisconnected;
            }
        }

        readonly VisualElement m_TabsContainer;
        readonly VisualElement m_Footer;

        readonly Dictionary<TabType, TabDetails> m_TabContents = new();

        readonly IUIPreferences m_UIPreferences;
        readonly IUnityConnectProxy m_UnityConnectProxy;

        bool m_IsFooterVisible;

        TabType ActiveTabType
        {
            get => (TabType)m_UIPreferences.GetInt("AssetDetailsPageTabs.ActiveTab", 0);
            set => m_UIPreferences.SetInt("AssetDetailsPageTabs.ActiveTab", (int)value);
        }

        public AssetDetailsPageTabs(VisualElement visualElement, VisualElement footer, IEnumerable<AssetTab> assetTabs)
        {
            m_TabsContainer = visualElement.Q("details-page-tabs");
            m_Footer = footer;

            foreach (var tab in assetTabs)
            {
                var button = new Button
                {
                    text = L10n.Tr(tab.Type.ToString())
                };
                button.clicked += () =>
                {
                    SetActiveTab(tab.Type);
                };
                button.focusable = false;
                m_TabsContainer.Add(button);

                m_TabContents[tab.Type] = new TabDetails(button, tab.Root, tab.IsFooterVisible, tab.EnabledWhenDisconnected);
            }

            m_UnityConnectProxy = ServicesContainer.instance.Resolve<IUnityConnectProxy>();
            m_UIPreferences = ServicesContainer.instance.Resolve<IUIPreferences>();

            SetActiveTab(ActiveTabType);
        }

        public void OnSelection(BaseAssetData assetData)
        {
            if (assetData.Identifier.IsLocal())
            {
                SetActiveTab(TabType.Details);
                UIElementsUtils.Hide(m_TabsContainer);
            }
            else
            {
                // This is not a local asset, you can show the tabs
                UIElementsUtils.Show(m_TabsContainer);
            }
        }

        public void RefreshUI(BaseAssetData assetData, bool isLoading = false)
        {
            foreach (var kvp in m_TabContents)
            {
                var button = kvp.Value.TabButton;
                if (m_UnityConnectProxy.AreCloudServicesReachable || kvp.Value.EnabledWhenDisconnected)
                {
                    button.SetEnabled(kvp.Key != ActiveTabType);
                    kvp.Value.TabButton.RemoveFromClassList("details-page-tabs-button--disabled");
                }
                else
                {
                    button.SetEnabled(false);
                    kvp.Value.TabButton.AddToClassList("details-page-tabs-button--disabled");
                }
            }

            if(!m_UnityConnectProxy.AreCloudServicesReachable && !m_TabContents[ActiveTabType].EnabledWhenDisconnected)
            {
                foreach (var kvp in m_TabContents)
                {
                    if (kvp.Value.EnabledWhenDisconnected)
                    {
                        SetActiveTab(kvp.Key);
                        return;
                    }
                }

                // If no tab is enabled when disconnected, hide the active content
                UIElementsUtils.Hide(m_TabContents[ActiveTabType].TabContent);
            }
            else if (m_UnityConnectProxy.AreCloudServicesReachable)
            {
                UIElementsUtils.Show(m_TabContents[ActiveTabType].TabContent);
            }
        }

        public void RefreshButtons(UIEnabledStates enabled, BaseAssetData assetData, BaseOperation operationInProgress)
        {
            UIElementsUtils.SetDisplay(m_Footer, m_IsFooterVisible && enabled.HasFlag(UIEnabledStates.CanImport));
        }

        void SetActiveTab(TabType activeTabType)
        {
            ActiveTabType = activeTabType;

            foreach (var kvp in m_TabContents)
            {
                var isActive = activeTabType == kvp.Key;
                kvp.Value.TabButton.SetEnabled(!isActive && (m_UnityConnectProxy.AreCloudServicesReachable || kvp.Value.EnabledWhenDisconnected));
                UIElementsUtils.SetDisplay(kvp.Value.TabContent, isActive);

                if (isActive)
                {
                    m_IsFooterVisible = kvp.Value.DisplayFooter;
                    UIElementsUtils.SetDisplay(m_Footer, m_IsFooterVisible);
                }
            }
        }
    }
}
