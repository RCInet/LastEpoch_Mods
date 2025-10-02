using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.Serialization;

namespace Unity.AssetManager.Core.Editor
{
    interface IProjectIconDownloader : IService
    {
        void DownloadIcon(string projectId, Action<string, Texture2D> doneCallbackAction = null);
    }

    [Serializable]
    class AllProjectResponse
    {
        public List<AllProjectResponseItem> Results { get; set; } = new();
    }

    [Serializable]
    class AllProjectResponseItem
    {
        public string Id { get; set; } = string.Empty;
        public string IconUrl { get; set; } = string.Empty;
    }

    [Serializable]
    class ProjectIconDownloader : BaseService<IProjectIconDownloader>, IProjectIconDownloader,
        ISerializationCallbackReceiver
    {
        [SerializeField]
        string[] m_SerializedIconsKeys;

        [SerializeField]
        Texture2D[] m_SerializedIcons;

        [SerializeReference]
        IPermissionsManager m_PermissionsManager;

        [SerializeReference]
        ICacheEvictionManager m_CacheEvictionManager;

        [SerializeReference]
        IDownloadManager m_DownloadManager;

        [SerializeReference]
        IUnityConnectProxy m_UnityConnectProxy;

        [SerializeReference]
        IIOProxy m_IOProxy;

        [SerializeReference]
        IProjectOrganizationProvider m_ProjectOrganizationProvider;

        [SerializeReference]
        ISettingsManager m_SettingsManager;

        const string k_TempExt = ".tmp";
        static readonly string k_ProjectIconCacheLocation = "ProjectIconCache";

        readonly Dictionary<ulong, string> m_DownloadIdToProjectIdMap = new();
        readonly Dictionary<string, List<Action<string, Texture2D>>> m_IconDownloadCallbacks = new();
        readonly Dictionary<string, Texture2D> m_Icons = new();
        readonly Dictionary<string, string> m_IconsUrls = new();

        OrganizationInfo m_OrganizationInfo;

        [ServiceInjection]
        public void Inject(IUnityConnectProxy unityConnectProxy, IDownloadManager downloadManager, IIOProxy ioProxy,
            ISettingsManager settingsManager, ICacheEvictionManager cacheEvictionManager,
            IProjectOrganizationProvider projectOrganizationProvider, IPermissionsManager permissionsManager)
        {
            m_UnityConnectProxy = unityConnectProxy;
            m_ProjectOrganizationProvider = projectOrganizationProvider;
            m_DownloadManager = downloadManager;
            m_IOProxy = ioProxy;
            m_SettingsManager = settingsManager;
            m_CacheEvictionManager = cacheEvictionManager;
            m_PermissionsManager = permissionsManager;
        }

        public override void OnEnable()
        {
            base.OnEnable();

            m_UnityConnectProxy.CloudServicesReachabilityChanged += OnCloudServicesReachabilityChanged;
            m_ProjectOrganizationProvider.OrganizationChanged += OnOrganizationChanged;
            m_DownloadManager.DownloadFinalized += OnDownloadFinalized;
            m_PermissionsManager.AuthenticationStateChanged += OnAuthenticationStateChanged;
        }

        protected override void ValidateServiceDependencies()
        {
            base.ValidateServiceDependencies();

            m_PermissionsManager ??= ServicesContainer.instance.Get<IPermissionsManager>();
        }

        public override void OnDisable()
        {
            m_UnityConnectProxy.CloudServicesReachabilityChanged -= OnCloudServicesReachabilityChanged;
            m_ProjectOrganizationProvider.OrganizationChanged -= OnOrganizationChanged;
            m_DownloadManager.DownloadFinalized -= OnDownloadFinalized;

            if (m_PermissionsManager != null)
                m_PermissionsManager.AuthenticationStateChanged -= OnAuthenticationStateChanged;
        }

        void OnAuthenticationStateChanged(AuthenticationState newState)
        {
            if (newState == AuthenticationState.LoggedIn
                && m_UnityConnectProxy.AreCloudServicesReachable
                && m_OrganizationInfo != null)
            {
                OnOrganizationChanged(m_OrganizationInfo);
            }
        }

        private void OnCloudServicesReachabilityChanged(bool cloudServicesReachable)
        {
            if (cloudServicesReachable && m_OrganizationInfo != null &&
                m_PermissionsManager.AuthenticationState == AuthenticationState.LoggedIn)
            {
                OnOrganizationChanged(m_OrganizationInfo);
            }
        }

        public void DownloadIcon(string projectId, Action<string, Texture2D> doneCallbackAction = null)
        {
            if (!m_IconsUrls.TryGetValue(projectId, out var iconUrl))
            {
                doneCallbackAction?.Invoke(projectId, null);
                return;
            }

            var iconFileName = Hash128.Compute(iconUrl).ToString();
            var icon = LoadIcon(iconUrl, Path.Combine(k_ProjectIconCacheLocation, iconFileName));
            if (icon != null)
            {
                doneCallbackAction?.Invoke(projectId, icon);
                return;
            }

            if (m_IconDownloadCallbacks.TryGetValue(iconUrl, out var callbacks))
            {
                callbacks.Add(doneCallbackAction);
                return;
            }

            var download = m_DownloadManager.CreateDownloadOperation<FileDownloadOperation>(iconUrl);
            download.Path = Path.Combine(m_SettingsManager.ThumbnailsCacheLocation, iconFileName + k_TempExt);

            m_DownloadManager.StartDownload(download);
            m_DownloadIdToProjectIdMap[download.Id] = projectId;
            var newCallbacks = new List<Action<string, Texture2D>>();
            if (doneCallbackAction != null)
            {
                newCallbacks.Add(doneCallbackAction);
            }

            m_IconDownloadCallbacks[iconUrl] = newCallbacks;
        }

        public void OnBeforeSerialize()
        {
            m_SerializedIconsKeys = m_Icons.Keys.ToArray();
            m_SerializedIcons = m_Icons.Values.ToArray();
        }

        public void OnAfterDeserialize()
        {
            for (var i = 0; i < m_SerializedIconsKeys.Length; i++)
            {
                m_Icons[m_SerializedIconsKeys[i]] = m_SerializedIcons[i];
            }
        }

        void OnDownloadFinalized(DownloadOperation operation)
        {
            if (!m_DownloadIdToProjectIdMap.Remove(operation.Id, out var projectId))
                return;

            if (!m_IconDownloadCallbacks.TryGetValue(operation.Url, out var callbacks) || callbacks.Count == 0)
                return;

            if (operation is not FileDownloadOperation fileDownloadOperation)
                return;

            var path = fileDownloadOperation.Path;

            var finalPath = path[..^k_TempExt.Length];
            m_IOProxy.DeleteFile(finalPath);
            m_IOProxy.FileMove(path, finalPath);

            var icon = LoadIcon(operation.Url, finalPath);
            m_CacheEvictionManager.OnCheckEvictConditions(finalPath);
            m_IconDownloadCallbacks.Remove(operation.Url);

            foreach (var callback in callbacks)
            {
                callback?.Invoke(projectId, icon);
            }
        }

        Texture2D LoadIcon(string url, string iconPath)
        {
            if (m_Icons.TryGetValue(url, out var result))
            {
                return result;
            }

            if (!m_IOProxy.FileExists(iconPath))
            {
                return null;
            }

            var texture2D = new Texture2D(1, 1);
            texture2D.LoadImage(m_IOProxy.FileReadAllBytes(iconPath));
            texture2D.hideFlags = HideFlags.HideAndDontSave;
            m_Icons[url] = texture2D;
            return texture2D;
        }

        async void OnOrganizationChanged(OrganizationInfo organizationInfo)
        {
            m_OrganizationInfo = organizationInfo;
            if (organizationInfo != null
                && m_UnityConnectProxy.AreCloudServicesReachable
                && m_PermissionsManager.AuthenticationState == AuthenticationState.LoggedIn)
            {
                var iconsUrls = await m_ProjectOrganizationProvider.GetProjectIconUrlsAsync(organizationInfo.Id, CancellationToken.None);

                if (iconsUrls == null)
                    return;

                foreach (var iconUrl in iconsUrls)
                {
                    m_IconsUrls[iconUrl.Key] = iconUrl.Value;
                }
            }
        }

        static readonly Color[] k_ProjectIconDefaultColors =
        {
            new Color32(233, 61, 130, 255), // Crimson
            new Color32(247, 107, 21, 255), // Orange
            new Color32(255, 166, 0, 255), // Amber
            new Color32(18, 165, 148, 255), // Teal
            new Color32(62, 99, 221, 255), // Indigo
            new Color32(110, 86, 207, 255), // Violet
        };

        public static readonly Color DefaultColor = new(40f / 255f, 40f / 255f, 40f / 255f);

        public static Color GetProjectIconColor(string projectId)
        {
            if (string.IsNullOrEmpty(projectId))
            {
                return k_ProjectIconDefaultColors[0];
            }

            var lastCharIndex = projectId.Length - 1;
            var lastCharCode = projectId[lastCharIndex];
            var colorIndex = lastCharCode % k_ProjectIconDefaultColors.Length;

            return k_ProjectIconDefaultColors[colorIndex];
        }
    }
}
