using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace Unity.AssetManager.Core.Editor
{
    interface IDownloadManager : IService
    {
        event Action<DownloadOperation> DownloadProgress;
        event Action<DownloadOperation> DownloadFinalized;

        T CreateDownloadOperation<T>(string url) where T : DownloadOperation, new();

        void StartDownload(DownloadOperation operation);
    }

    [Serializable]
    class DownloadManager : BaseService<IDownloadManager>, IDownloadManager
    {
        [SerializeReference]
        IApplicationProxy m_ApplicationProxy;

        [SerializeField]
        ulong m_LastDownloadOperationId;

        [SerializeReference]
        List<DownloadOperation> m_PendingDownloads = new();

        readonly List<DownloadOperation> m_DownloadInProgress = new();
        readonly Dictionary<ulong, IWebRequestItem> m_WebRequests = new();

        const int k_MaxConcurrentDownloads = 10;
        const int k_MaxFrameDurationMS = 20;

        System.Diagnostics.Stopwatch m_Stopwatch = new();

        public event Action<DownloadOperation> DownloadProgress = delegate { };
        public event Action<DownloadOperation> DownloadFinalized = delegate { };

        [ServiceInjection]
        public void Inject(IApplicationProxy applicationProxy)
        {
            m_ApplicationProxy = applicationProxy;
        }

        public override void OnEnable()
        {
            base.OnEnable();

            m_ApplicationProxy.Update += Update;
        }

        protected override void ValidateServiceDependencies()
        {
            base.ValidateServiceDependencies();

            m_ApplicationProxy ??= ServicesContainer.instance.Get<IApplicationProxy>();
        }

        public override void OnDisable()
        {
            // Maintaining for backwards compatibility before IApplicationProxy was serialized
            EditorApplication.update -= Update;
            if (m_ApplicationProxy != null)
                m_ApplicationProxy.Update -= Update;
        }

        public T CreateDownloadOperation<T>(string url) where T : DownloadOperation, new()
        {
            return new T
            {
                Id = ++m_LastDownloadOperationId,
                Url = url
            };
        }

        public void StartDownload(DownloadOperation operation)
        {
            if (m_PendingDownloads.Contains(operation)
                || m_DownloadInProgress.Contains(operation))
            {
                return;
            }

            m_PendingDownloads.Add(operation);
        }

        void Update()
        {
            m_Stopwatch.Restart();

            var numDownloadsToAdd = Math.Min(k_MaxConcurrentDownloads - m_DownloadInProgress.Count,
                m_PendingDownloads.Count);
            if (numDownloadsToAdd > 0)
            {
                var initializedOperations = new List<DownloadOperation>();
                try
                {
                    foreach (var operation in m_PendingDownloads.Take(numDownloadsToAdd))
                    {
                        InitializeOperation(operation);
                        initializedOperations.Add(operation);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }

                m_PendingDownloads.RemoveAll(op => initializedOperations.Contains(op));
            }

            if (m_DownloadInProgress.Count == 0)
                return;

            var operations = m_DownloadInProgress.ToArray();

            foreach (var operation in operations)
            {
                UpdateOperation(operation);

                if (operation.Status != OperationStatus.InProgress)
                {
                    m_DownloadInProgress.Remove(operation);
                }

                // Avoid spending too much time in a single frame and slow down the UI
                if (m_Stopwatch.ElapsedMilliseconds > k_MaxFrameDurationMS)
                {
                    break;
                }
            }
        }

        void InitializeOperation(DownloadOperation operation)
        {
            var newRequest = operation.SendWebRequest();

            m_WebRequests[operation.Id] = newRequest;
            m_DownloadInProgress.Add(operation);
            operation.Start();
        }

        void FinalizeOperation(DownloadOperation operation, IWebRequestItem request, OperationStatus finalStatus,
            string errorMessage = null)
        {
            if (!string.IsNullOrEmpty(errorMessage))
            {
                Utilities.DevLogError($"Encountered error while downloading {operation.Description}: {errorMessage}");
            }

            if (finalStatus == OperationStatus.Success)
            {
                operation.SetProgress(1f);
            }

            operation.Finish(finalStatus);

            m_WebRequests.Remove(operation.Id);
            request?.Dispose();

            DownloadFinalized.Invoke(operation);
        }

        void UpdateOperation(DownloadOperation operation)
        {
            if (!m_WebRequests.TryGetValue(operation.Id, out var request))
                return;

            if (!string.IsNullOrEmpty(request.Error))
            {
                FinalizeOperation(operation, request, OperationStatus.Error, request.Error);
                return;
            }

            switch (request.Result)
            {
                case UnityWebRequest.Result.ConnectionError:
                    FinalizeOperation(operation, request, OperationStatus.Error,
                        "Failed to communicate with the server.");
                    return;
                case UnityWebRequest.Result.ProtocolError:
                    FinalizeOperation(operation, request, OperationStatus.Error,
                        "The server returned an error response.");
                    return;
                case UnityWebRequest.Result.DataProcessingError:
                    FinalizeOperation(operation, request, OperationStatus.Error, "Error processing data.");
                    return;
                case UnityWebRequest.Result.InProgress:
                case UnityWebRequest.Result.Success:
                default:
                    break;
            }

            if (request.IsDone)
            {
                FinalizeOperation(operation, request, OperationStatus.Success);
                return;
            }

            var progressUpdate = request.DownloadProgress - operation.Progress;

            // We are reducing how often we are reporting download progress to avoid expensive frequent UI refreshes.
            if (progressUpdate >= 0.05 || progressUpdate * operation.TotalBytes > 1024 * 1024)
            {
                operation.SetProgress(request.DownloadProgress);
                DownloadProgress?.Invoke(operation);
            }
        }
    }
}
