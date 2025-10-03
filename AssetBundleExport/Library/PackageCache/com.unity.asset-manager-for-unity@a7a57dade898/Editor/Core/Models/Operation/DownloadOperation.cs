using System;
using UnityEngine.Networking;

namespace Unity.AssetManager.Core.Editor
{
    [Serializable]
    class FileDownloadOperation : DownloadOperation
    {
        public string Path;
        public override string Description => $"{System.IO.Path.GetFileName(Path)}";

        protected override DownloadHandler GetDownloadHandler()
        {
            return new DownloadHandlerFile(Path) { removeFileOnAbort = true };
        }
    }

    [Serializable]
    abstract class DownloadOperation : BaseOperation
    {
        public ulong Id;
        public string Url;

        public long TotalBytes;

        float m_Progress;

        public override float Progress => m_Progress;
        public override string OperationName => "Downloading";
        public abstract override string Description { get; }
        public IWebRequestItem RequestItem { get; protected set; }

        protected UnityWebRequest UnityWebRequest;

        public IWebRequestItem SendWebRequest()
        {
            Utilities.DevAssert(RequestItem == null);
            UnityWebRequest = new UnityWebRequest(Url, UnityWebRequest.kHttpVerbGET) { disposeDownloadHandlerOnDispose = true };

            UnityWebRequest.downloadHandler = GetDownloadHandler();
            UnityWebRequest.SendWebRequest();
            RequestItem = new WebRequestItem(UnityWebRequest);
            return RequestItem;
        }

        public void SetProgress(float progress)
        {
            m_Progress = progress;
            Report();
        }

        protected abstract DownloadHandler GetDownloadHandler();
    }
}
