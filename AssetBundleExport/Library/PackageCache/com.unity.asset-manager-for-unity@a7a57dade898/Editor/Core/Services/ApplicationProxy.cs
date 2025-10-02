using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

namespace Unity.AssetManager.Core.Editor
{
    interface IApplicationProxy : IService
    {
        RuntimePlatform Platform { get; }
        string DataPath { get; }
        bool InternetReachable { get; }
        double TimeSinceStartup { get; }
        bool IsBatchMode { get; }

        event EditorApplication.CallbackFunction Update;
        event EditorApplication.CallbackFunction DelayCall;

        void OpenUrl(string url);
    }

    [Serializable]
    [ExcludeFromCoverage]
    class ApplicationProxy : BaseService<IApplicationProxy>, IApplicationProxy
    {
        public RuntimePlatform Platform => Application.platform;
        public string DataPath => Application.dataPath;
        public bool InternetReachable => Application.internetReachability != NetworkReachability.NotReachable;
        public double TimeSinceStartup => EditorApplication.timeSinceStartup;
        public bool IsBatchMode => Application.isBatchMode;

        public event EditorApplication.CallbackFunction Update
        {
            add => EditorApplication.update += value;
            remove => EditorApplication.update -= value;
        }

        public event EditorApplication.CallbackFunction DelayCall
        {
            add => EditorApplication.delayCall += value;
            remove => EditorApplication.delayCall -= value;
        }

        public void OpenUrl(string url) => Application.OpenURL(url);
    }
}
