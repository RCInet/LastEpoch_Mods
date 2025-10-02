using System;
using Unity.AssetManager.Core.Editor;
using Unity.AssetManager.Upload.Editor;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

namespace Unity.AssetManager.UI.Editor
{
    class AssetManagerWindow : EditorWindow, IHasCustomMenu
    {
        static readonly Vector2 k_MinWindowSize = new(600, 250);
        static AssetManagerWindow s_Instance;

        bool m_IsDocked;
        AssetManagerWindowRoot m_Root;
        DragFromOutsideManipulator m_Manipulator;

        public static AssetManagerWindow Instance => s_Instance;

        [MenuItem("Window/Asset Manager", priority = 1500)]
        static void MenuEntry()
        {
            Open();

            // Hack - We don't want to show the UploadPage when the window is opened from the menu
            var pageManager = ServicesContainer.instance.Resolve<IPageManager>();
            if (pageManager?.ActivePage is UploadPage)
            {
                pageManager.SetActivePage<CollectionPage>();
            }
        }

        void CreateGUI()
        {
            if (s_Instance == null)
            {
                s_Instance = this;
            }

            if (s_Instance != this)
                return;

            m_IsDocked = docked;

            var container = ServicesContainer.instance;

            m_Root = new AssetManagerWindowRoot(
                container.Resolve<IPageManager>(),
                container.Resolve<IAssetDataManager>(),
                container.Resolve<IAssetImporter>(),
                container.Resolve<IAssetOperationManager>(),
                container.Resolve<IStateManager>(),
                container.Resolve<IUnityConnectProxy>(),
                container.Resolve<IProjectOrganizationProvider>(),
                container.Resolve<ILinksProxy>(),
                container.Resolve<IAssetDatabaseProxy>(),
                container.Resolve<IProjectIconDownloader>(),
                container.Resolve<IPermissionsManager>(),
                container.Resolve<IUploadManager>(),
                container.Resolve<IPopupManager>(),
                container.Resolve<IAssetImportResolver>(),
                container.Resolve<IMessageManager>(),
                container.Resolve<IApplicationProxy>(),
                container.Resolve<IDialogManager>(),
                container.Resolve<ISettingsManager>(),
                container.Resolve<ISavedAssetSearchFilterManager>());

            m_Root.RegisterCallback<GeometryChangedEvent>(OnResized);
            m_Root.OnEnable();
            m_Root.StretchToParentSize();
            rootVisualElement.Add(m_Root);

            // Manipulators and Inputs
            m_Manipulator = new DragFromOutsideManipulator(rootVisualElement, container.Resolve<IPageManager>(),
                container.Resolve<IUploadManager>());
            rootVisualElement.RegisterCallback<KeyDownEvent>(OnKeyDown);
            // This line is needed in order to receive the KeyDownEvent
            rootVisualElement.focusable = true;

            AnalyticsSender.SendEvent(new ServicesInitializationCompletedEvent(position.size));
            if (docked)
            {
                AnalyticsSender.SendEvent(new WindowDockedEvent(true));
            }

            // This need to be done in OnEnable to ensure the icon is the right color when switching between dark and light mode
            titleContent = new GUIContent("Asset Manager", UIElementsUtils.GetPackageIcon());

            Enabled?.Invoke();
        }

        void OnDisable()
        {
            if (s_Instance == null)
            {
                s_Instance = this;
            }

            if (s_Instance != this)
                return;

            m_Manipulator?.target.RemoveManipulator(m_Manipulator);
            rootVisualElement.UnregisterCallback<KeyDownEvent>(OnKeyDown);

            m_Root?.UnregisterCallback<GeometryChangedEvent>(OnResized);
            m_Root?.OnDisable();

            // Disable the service if the AM window is closed
            ServicesContainer.instance.OnDisable();
        }

        void OnDestroy()
        {
            if (rootVisualElement.Contains(m_Root))
            {
                rootVisualElement.Remove(m_Root);
            }

            s_Instance = null;
        }

        void OnFocus()
        {
            if (m_Root != null && m_Root.CurrentOrganizationIsEmpty())
            {
                RefreshAll();
            }
        }

        public void AddItemsToMenu(GenericMenu menu)
        {
            var refreshItem = new GUIContent("Refresh");
            menu.AddItem(refreshItem, false, Refresh);

            m_Root?.AddItemsToMenu(menu);
        }

        public static event Action Enabled;

        // This event is used for the Tests
        internal static event Action Refreshed;

        internal static void Open()
        {
            var window = GetWindow<AssetManagerWindow>();
            window.minSize = k_MinWindowSize;
            window.Show();
        }

        internal void RefreshAll()
        {
            Refreshed?.Invoke();

            // Calling a manual Refresh should force a brand new initialization of the services and UI
            OnDisable();
            OnDestroy();

            ServicesInitializer.ResetServices();

            CreateGUI();
        }

        void Refresh()
        {
            RefreshAll();

            AnalyticsSender.SendEvent(new MenuItemSelectedEvent(MenuItemSelectedEvent.MenuItemType.Refresh));
        }

        void OnResized(GeometryChangedEvent evt)
        {
            if (docked == m_IsDocked)
                return;

            m_IsDocked = docked;
            AnalyticsSender.SendEvent(new WindowDockedEvent(docked));
        }

        void OnKeyDown(KeyDownEvent evt)
        {
            switch (evt.keyCode)
            {
                case KeyCode.Escape:
                    if (evt.modifiers == EventModifiers.None)
                    {
                        ServicesContainer.instance.Resolve<IPageManager>().ActivePage.Clear(true);
                    }
                    break;
            }
        }
    }
}
