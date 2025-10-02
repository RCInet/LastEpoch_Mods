using System;
using Unity.AssetManager.Core.Editor;

namespace Unity.AssetManager.UI.Editor
{
    class AssetManagerWindowHook
    {
        public event Action WindowEnabled;
        public event Action OrganizationLoaded;

        public void OpenAssetManagerWindow()
        {
            if (AssetManagerWindow.Instance == null)
            {
                AssetManagerWindow.Enabled += OnWindowEnabled;
                AssetManagerWindow.Open();
            }
            else
            {
                WindowEnabled?.Invoke();
                OnWindowEnabled();
            }
        }

        void OnWindowEnabled()
        {
            AssetManagerWindow.Enabled -= OnWindowEnabled;

            var provider = ServicesContainer.instance.Resolve<IProjectOrganizationProvider>();

            if (provider.SelectedOrganization == null)
            {
                provider.OrganizationChanged += OnOrganizationLoaded;
            }
            else
            {
                OnOrganizationLoaded(provider.SelectedOrganization);
            }
        }

        void OnOrganizationLoaded(OrganizationInfo organization)
        {
            if (organization?.Id == null)
                return;

            var provider = ServicesContainer.instance.Resolve<IProjectOrganizationProvider>();
            provider.OrganizationChanged -= OnOrganizationLoaded;

            OrganizationLoaded?.Invoke();
        }
    }
}
