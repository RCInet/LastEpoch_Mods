using System;

namespace Unity.AssetManager.Upload.Editor
{
    interface IUploadStagingStatus
    {
        string TargetOrganizationId { get; }
        string TargetProjectId { get; }
        int IgnoredAssetCount { get; }
        int SkippedAssetCount { get; }
        int UpdatedAssetCount { get; }
        int AddedAssetCount { get; }
        int TotalAssetCount { get; }
        int ManuallyIgnoredDependencyCount { get; }
        int ReadyAssetCount { get; }
        bool HasFilesOutsideProject { get; }
        int TotalFileCount { get; }
        long TotalSize { get; }
    }

    [Serializable]
    class UploadStagingStatus : IUploadStagingStatus
    {
        public string TargetOrganizationId { get; private set; }
        public string TargetProjectId { get; private set; }

        public int IgnoredAssetCount { get; set; }
        public int SkippedAssetCount { get; set; }
        public int UpdatedAssetCount { get; set; }
        public int AddedAssetCount { get; set; }
        public int TotalAssetCount { get; set; }
        public int ManuallyIgnoredDependencyCount { get; set; }
        public int ReadyAssetCount { get; set; }
        public bool HasFilesOutsideProject { get; set; }
        public int TotalFileCount { get; set; }
        public long TotalSize { get; set; }

        public UploadStagingStatus(string targetOrganizationId, string targetProjectId)
        {
            TargetOrganizationId = targetOrganizationId;
            TargetProjectId = targetProjectId;
        }
    }
}
