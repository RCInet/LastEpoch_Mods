using System;
using UnityEngine.Networking;

namespace Unity.AssetManager.Core.Editor
{
    interface IWebRequestItem
    {
        bool IsDone { get; }
        float DownloadProgress { get; }
        string Error { get; }
        UnityWebRequest.Result Result { get; }

        void Abort();
        void Dispose();
    }

    class WebRequestItem : IWebRequestItem
    {
        readonly UnityWebRequest m_UnityWebRequest;

        public bool IsDone => m_UnityWebRequest.isDone;
        public float DownloadProgress => m_UnityWebRequest.downloadProgress;
        public string Error => m_UnityWebRequest.error;
        public UnityWebRequest.Result Result => m_UnityWebRequest.result;

        public WebRequestItem(UnityWebRequest unityWebRequest)
        {
            m_UnityWebRequest = unityWebRequest;
        }

        public void Abort()
        {
            m_UnityWebRequest.Abort();
        }

        public void Dispose()
        {
            m_UnityWebRequest.Dispose();
        }
    }
}
