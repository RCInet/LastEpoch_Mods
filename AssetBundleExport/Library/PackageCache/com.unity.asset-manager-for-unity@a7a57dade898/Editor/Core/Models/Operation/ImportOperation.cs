using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Unity.AssetManager.Core.Editor
{
    [Serializable]
    class ImportOperation : AssetDataOperation
    {
        public enum ImportType
        {
            Import,
            UpdateToLatest
        }

        public interface IFileDownload
        {
            public string DownloadPath { get; }
            public string OriginalPath { get; }
            public string Error { get; }
        }

        class FileDownloadRequests : IFileDownload
        {
            public string DownloadPath { get; }
            public string OriginalPath { get; }
            public string Error => m_WebRequest.error;

            public UnityWebRequest WebRequest => m_WebRequest;

            readonly UnityWebRequest m_WebRequest;

            public FileDownloadRequests(string downloadPath, string originalPath, UnityWebRequest webRequest)
            {
                DownloadPath = downloadPath;
                OriginalPath = originalPath;
                m_WebRequest = webRequest;
            }
        }

        List<FileDownloadRequests> m_DownloadRequests = new();

        [SerializeReference]
        BaseAssetData m_AssetData;
        [SerializeReference]
        ISettingsManager m_SettingsManager;

        readonly DateTime m_StartTime;
        readonly string m_TempDownloadPath;
        readonly Dictionary<string, string> m_DestinationPathPerFile;
        readonly string m_DefaultDestinationPath;

        public DateTime StartTime => m_StartTime;
        public string TempDownloadPath => m_TempDownloadPath;

        public BaseAssetData AssetData => m_AssetData;
        public IReadOnlyCollection<IFileDownload> DownloadRequests => m_DownloadRequests;

        public override AssetIdentifier Identifier => m_AssetData?.Identifier;
        public override string OperationName => $"Importing {m_AssetData?.Name}";
        public override string Description => DownloadRequests.Count > 0 ? "Downloading files..." : "Preparing download...";
        public override bool StartIndefinite => true;
        public override bool IsSticky => true;

        public override float Progress
        {
            get
            {
                var totalProgress = 0f;

                foreach (var download in m_DownloadRequests)
                {
                    totalProgress += Mathf.Max(download.WebRequest.downloadProgress, 0.0f); // Non started web request will return progress of -1
                }

                return m_DownloadRequests.Count > 0 ? totalProgress / m_DownloadRequests.Count : 0f;
            }
        }

        public override void Finish(OperationStatus status)
        {
            base.Finish(status);

            if (status == OperationStatus.Success)
            {
                Remove();
            }
        }

        float m_LastReportedProgress = 0f;

        public ImportOperation(BaseAssetData assetData, string tempDownloadPath, Dictionary<string, string> destinationPathPerFile, string defaultDestinationPath, ISettingsManager settingsManager)
        {
            m_AssetData = assetData;
            m_TempDownloadPath = tempDownloadPath;
            m_DestinationPathPerFile = destinationPathPerFile;
            m_StartTime = DateTime.Now;
            m_DefaultDestinationPath = defaultDestinationPath;
            m_SettingsManager = settingsManager;
        }

        public async Task ImportAsync(CancellationToken token = default)
        {
            if (m_AssetData == null)
                return;

            if (IsDebugLogsEnabled())
            {
                Debug.Log($"Retrieving download URLs for asset: {m_AssetData.Identifier.ToString()}");
            }

            var assetsProvider = ServicesContainer.instance.Resolve<IAssetsProvider>();

            var fetchDownloadUrlsOperation = new FetchDownloadUrlsOperation();
            fetchDownloadUrlsOperation.Start();

            try
            {
                var tasks = new List<Task<IReadOnlyDictionary<string, Uri>>>();

                foreach (var dataset in m_AssetData.Datasets.Where(x => x.CanBeImported))
                {
                    tasks.Add(assetsProvider.GetDatasetDownloadUrlsAsync(m_AssetData.Identifier, dataset, fetchDownloadUrlsOperation, token));
                }

                var results = await Task.WhenAll(tasks);

                var downloadUrls = results.SelectMany(x => x).ToDictionary(x => x.Key, x => x.Value);

                fetchDownloadUrlsOperation.Finish(OperationStatus.Success);

                if (downloadUrls.Count == 0)
                {
                    if (IsDebugLogsEnabled())
                    {
                        Debug.Log("Nothing to download from asset: " + m_AssetData.Identifier.AssetId);
                    }

                    Utilities.DevLog($"Nothing to download from asset '{m_AssetData?.Name}'.");
                    Finish(OperationStatus.Success);
                    return;
                }

                foreach (var (filePath, url) in downloadUrls)
                {
                    if (url == null)
                    {
                        Utilities.DevLogWarning($"Skipping downloading null url for file '{filePath}'.");
                        continue;
                    }

                    // If the file is already in the project, use the existing path in case it was moved
                    if (!m_DestinationPathPerFile.TryGetValue(filePath, out var destinationPath))
                    {
                        // If the file is not found, it might be a metafile, so we try to find the destination path without the meta extension
                        if (MetafilesHelper.IsMetafile(filePath) &&
                            m_DestinationPathPerFile.TryGetValue(MetafilesHelper.RemoveMetaExtension(filePath), out var destinationPathWithoutMeta))
                        {
                            destinationPath = $"{destinationPathWithoutMeta}{MetafilesHelper.MetaFileExtension}";
                        }
                        else
                        {
                            // Otherwise, we use the path from the default destination path
                            destinationPath = Path.Combine(m_DefaultDestinationPath, filePath);
                        }
                    }

                    var tempPath = Path.Combine(TempDownloadPath, destinationPath);
                    m_DownloadRequests.Add(new FileDownloadRequests(tempPath, filePath, CreateFileDownloadRequest(url.AbsoluteUri, tempPath)));
                }
            }
            catch (TaskCanceledException)
            {
                fetchDownloadUrlsOperation.Finish(OperationStatus.None);
                throw;
            }

            if (m_DownloadRequests.Count == 0)
            {
                throw new InvalidOperationException($"Nothing to download from asset '{m_AssetData?.Name}'. Asset is empty, unavailable or corrupted.");
            }
            else
            {
                if (IsDebugLogsEnabled())
                {
                    Debug.Log(
                        $"Found {m_DownloadRequests.Count} files to download for asset: {m_AssetData.Identifier.ToString()}");
                }
            }
        }


        static UnityWebRequest CreateFileDownloadRequest(string url, string path)
        {
            var request = new UnityWebRequest(url, UnityWebRequest.kHttpVerbGET)
                { disposeDownloadHandlerOnDispose = true };

            request.downloadHandler = new DownloadHandlerFile(path, false) { removeFileOnAbort = true };

            return request;
        }

        public async Task StartDownloadRequests()
        {
            if (IsDebugLogsEnabled())
            {
                Debug.Log($"Starting downloads for asset: {m_AssetData.Identifier.ToString()}");
            }

            if (Status is not OperationStatus.InProgress)
                return;

            var downloadOperation = new FileDownloadOperation();
            downloadOperation.Start();
            var operations = m_DownloadRequests.Select(r => r.WebRequest.SendWebRequest()).ToList();

            while (operations.Exists(x => !x.isDone))
            {
                if (Progress - m_LastReportedProgress > 0.01)
                {
                    m_LastReportedProgress = Progress;
                    downloadOperation.SetProgress(Progress);
                    Report();
                }

                await Task.Delay(200);
            }

            if (Status == OperationStatus.Cancelled) // got cancelled while waiting for download requests
            {
                downloadOperation.Finish(OperationStatus.Cancelled);
                Finish(OperationStatus.Cancelled);
                return;
            }

            var status = m_DownloadRequests.Exists(x => !string.IsNullOrEmpty(x.WebRequest.error))
                ? OperationStatus.Error
                : OperationStatus.Success;

            downloadOperation.Finish(status);

            Finish(status);

            if (IsDebugLogsEnabled())
            {
                if (status == OperationStatus.Error)
                {
                    Debug.LogError(
                        $"The following import operation(s) for asset {m_AssetData.Identifier.AssetId} failed:\n{string.Join("\n", m_DownloadRequests.Where(x => !string.IsNullOrEmpty(x.WebRequest.error)).Select(x => $"{x.OriginalPath} - {x.Error}"))}");
                }
            }
        }

        bool IsDebugLogsEnabled()
        {
            return m_SettingsManager != null && m_SettingsManager.IsDebugLogsEnabled;
        }
    }
}
