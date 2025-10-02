using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.AssetManager.Core.Editor;
using UnityEngine;
using UnityEngine.UIElements;
using AuthenticationState = Unity.AssetManager.Core.Editor.AuthenticationState;

namespace Unity.AssetManager.UI.Editor
{
    class SideBarOrganizationSelector : VisualElement
    {
        const string k_UssClassName = "unity-org-selector";
        const string k_DropdownUssClassName = k_UssClassName + "-dropdown";
        const string k_DefaultOrgTooltip = "If configured, the Organization linked within " +
            "the Project Settings will display at the top of the Organization List.";

        [SerializeReference]
        IPermissionsManager m_PermissionsManager;

        [SerializeReference]
        IProjectOrganizationProvider m_ProjectOrganizationProvider;

        [SerializeReference]
        IUnityConnectProxy m_UnityConnectProxy;

        SeparatorDropdownMenu m_OrganizationDropdownMenu;

        Dictionary<string, NameAndId> m_OrganizationOptions = new();

        public SideBarOrganizationSelector(IPermissionsManager permissionsManager, IProjectOrganizationProvider projectOrganizationProvider, IUnityConnectProxy unityConnectProxy)
        {
            m_PermissionsManager = permissionsManager;
            m_ProjectOrganizationProvider = projectOrganizationProvider;
            m_UnityConnectProxy = unityConnectProxy;

            m_PermissionsManager.AuthenticationStateChanged += OnAuthenticationStateChanged;
            m_ProjectOrganizationProvider.OrganizationChanged += OnOrganizationChanged;
            m_UnityConnectProxy.OrganizationIdChanged += RefreshDropdown;

            InitializeUI();
        }

        void OnAuthenticationStateChanged(AuthenticationState authenticationState)
        {
            if (authenticationState == AuthenticationState.LoggedIn)
            {
                m_OrganizationDropdownMenu.style.display = DisplayStyle.Flex;
                _ = PopulateDropdown();
            }
            else
            {
                ClearDropdown();
                m_OrganizationDropdownMenu.SetEnabled(false);
                m_OrganizationDropdownMenu.style.display = DisplayStyle.None;
            }

        }

        void OnOrganizationChanged(OrganizationInfo _)
        {
            RefreshDropdown();
        }

        void RefreshDropdown()
        {
            _ = PopulateDropdown();
        }

        void InitializeUI()
        {
            AddToClassList(k_UssClassName);

            m_OrganizationDropdownMenu = new SeparatorDropdownMenu(tooltip: k_DefaultOrgTooltip);
            m_OrganizationDropdownMenu.AddToClassList(k_DropdownUssClassName);
            m_OrganizationDropdownMenu.RegisterValueChangedCallback(OnSelectionChanged);
            Add(m_OrganizationDropdownMenu);

            if (m_PermissionsManager.AuthenticationState == AuthenticationState.LoggedIn)
                _ = PopulateDropdown();
        }

        void OnSelectionChanged(string organizationName)
        {
            if (m_OrganizationOptions.TryGetValue(organizationName, out var organization))
                m_ProjectOrganizationProvider.SelectOrganization(organization.Id);

            AnalyticsSender.SendEvent(new OrganizationSelectedEvent());
        }

        async Task PopulateDropdown()
        {
            ClearDropdown();

            m_OrganizationOptions = new Dictionary<string, NameAndId>();
            await foreach (var organization in m_ProjectOrganizationProvider.ListOrganizationsAsync())
                m_OrganizationOptions[organization.Name] = organization;

            var multipleOrgOptions = m_OrganizationOptions.Count > 1;

            var linkedOrganizationId = m_UnityConnectProxy.HasValidOrganizationId ? m_UnityConnectProxy.OrganizationId : null;
            var linkedOrganizationName = string.Empty;
            if (linkedOrganizationId != null)
                linkedOrganizationName = m_OrganizationOptions.Values.FirstOrDefault(o => o.Id == linkedOrganizationId).Name;

            // The linked organization should be first in the list, with a separator to the rest of the options
            var dropdownMenuItems = new List<DropdownMenuItem>();
            if (!string.IsNullOrWhiteSpace(linkedOrganizationName))
            {
                dropdownMenuItems.Add(new DropdownMenuItem(linkedOrganizationName));

                if (multipleOrgOptions)
                    dropdownMenuItems.Add(new DropdownMenuItem(string.Empty, true));
            }

            if (!multipleOrgOptions)
            {
                m_OrganizationDropdownMenu.SetDropdownEnabled(false);
            }
            else
            {
                foreach (var organizationName in m_OrganizationOptions.Keys.OrderBy(organization => organization).ToList())
                {
                    if (organizationName != linkedOrganizationName)
                        dropdownMenuItems.Add(new DropdownMenuItem(organizationName));
                }

                m_OrganizationDropdownMenu.choices = dropdownMenuItems;
                m_OrganizationDropdownMenu.SetEnabled(true);
                m_OrganizationDropdownMenu.SetDropdownEnabled(true);
            }


            var selectedOrganization = m_ProjectOrganizationProvider.SelectedOrganization;
            if (selectedOrganization != null)
                SetSelectedOrganizationWithoutNotify(selectedOrganization.Name);
        }

        void ClearDropdown()
        {
            m_OrganizationOptions.Clear();
            m_OrganizationDropdownMenu.choices = new();
            m_OrganizationDropdownMenu.SetValueWithoutNotify(string.Empty);
        }

        void SetSelectedOrganizationWithoutNotify(string organizationName)
        {
            if (m_OrganizationOptions.ContainsKey(organizationName))
                    m_OrganizationDropdownMenu.SetValueWithoutNotify(organizationName);
        }
    }
}
