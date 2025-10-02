using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Unity.AssetManager.Core.Editor
{
    interface IThumbnailDownloader : IService
    {
        void DownloadThumbnail(AssetIdentifier identifier, string url,
            Action<AssetIdentifier, Texture2D> doneCallbackAction = null);

        Texture2D GetCachedThumbnail(AssetIdentifier identifier);
    }

    [Serializable]
    class ThumbnailDownloader : BaseService<IThumbnailDownloader>, IThumbnailDownloader
    {
        [SerializeReference]
        IIOProxy m_IOProxy;

        [SerializeReference]
        ISettingsManager m_SettingsManager;

        readonly Dictionary<string, AssetIdentifier> m_DownloadIdToAssetIdMap = new();
        readonly Dictionary<string, List<Action<AssetIdentifier, Texture2D>>> m_ThumbnailDownloadCallbacks = new();

        [ServiceInjection]
        public void Inject(IIOProxy ioProxy, ISettingsManager settingsManager)
        {
            m_IOProxy = ioProxy;
            m_SettingsManager = settingsManager;
        }

        public Texture2D GetCachedThumbnail(AssetIdentifier identifier)
        {
            // Try load from disk
            return LoadThumbnailFromDisk(identifier);
        }

        public void DownloadThumbnail(AssetIdentifier identifier, string url, Action<AssetIdentifier, Texture2D> doneCallbackAction = null)
        {
            if (string.IsNullOrEmpty(url))
            {
                doneCallbackAction?.Invoke(identifier, null);
                return;
            }

            // Hash the URL to get a unique identifier for the download
            var urlHash = GetHashKey(url);

            // Check if the thumbnail is already being downloaded
            if (m_ThumbnailDownloadCallbacks.TryGetValue(url, out var callbacks))
            {
                callbacks.Add(doneCallbackAction);
                return;
            }

            // Map the URL hash to the identifier to track started downloads
            m_DownloadIdToAssetIdMap[urlHash] = identifier;

            DownloadThumbnail(url, urlHash);

            var newCallbacks = new List<Action<AssetIdentifier, Texture2D>>();
            if (doneCallbackAction != null)
            {
                newCallbacks.Add(doneCallbackAction);
            }

            m_ThumbnailDownloadCallbacks[urlHash] = newCallbacks;
        }

        void DownloadThumbnail(string url, string downloadId)
        {
            var unityWebRequest = new UnityWebRequest(url, UnityWebRequest.kHttpVerbGET)
                {disposeDownloadHandlerOnDispose = true};
            unityWebRequest.downloadHandler = new DownloadHandlerTexture();

            var webRequestAsyncOperation = unityWebRequest.SendWebRequest();
            webRequestAsyncOperation.completed += asyncOp =>
            {
                var webOperation = (UnityWebRequestAsyncOperation) asyncOp;
                OnRequestCompletion(webOperation.webRequest, downloadId);
            };
        }

        void OnRequestCompletion(UnityWebRequest webRequest, string downloadId)
        {
            if (!m_DownloadIdToAssetIdMap.Remove(downloadId, out var assetIdentifier))
                return;

            if (!m_ThumbnailDownloadCallbacks.TryGetValue(downloadId, out var callbacks) || callbacks.Count == 0)
                return;

            m_ThumbnailDownloadCallbacks.Remove(downloadId);

            var isSuccess = webRequest.result == UnityWebRequest.Result.Success;
            Texture2D thumbnail = null;

            if (isSuccess)
            {
                thumbnail = DownloadHandlerTexture.GetContent(webRequest);
            }
            else
            {
                Utilities.DevLogError("Unable to download thumbnail. Error: " + webRequest.error);
            }

            foreach (var callback in callbacks)
            {
                callback?.Invoke(assetIdentifier, thumbnail);
            }

            if (isSuccess)
            {
                SaveThumbnailToDisk(thumbnail, assetIdentifier);
            }
        }

        void SaveThumbnailToDisk(Texture2D texture, AssetIdentifier assetIdentifier)
        {
            var path = GetThumbnailPath(assetIdentifier);

            // If the texture is null, delete the file
            if (texture == null)
            {
                m_IOProxy.DeleteFile(path);
            }

            var bytes = texture.EncodeToPNG();
            Task.Run(() => m_IOProxy.FileWriteAllBytes(path, bytes));
        }

        Texture2D LoadThumbnailFromDisk(AssetIdentifier assetIdentifier)
        {
            var path = GetThumbnailPath(assetIdentifier);

            if (!m_IOProxy.FileExists(path))
            {
                return null;
            }

            var texture2D = new Texture2D(1, 1);
            texture2D.LoadImage(m_IOProxy.FileReadAllBytes(path));

            return texture2D;
        }

        string GetHashKey(string url)
        {
            return Hash128.Compute(url).ToString();
        }

        string GetHashKey(AssetIdentifier assetIdentifier)
        {
            return Hash128.Compute(assetIdentifier.ToString()).ToString();
        }

        string GetThumbnailPath(AssetIdentifier assetIdentifier)
        {
            return Path.Combine(m_SettingsManager.ThumbnailsCacheLocation, GetHashKey(assetIdentifier));
        }
    }
}
