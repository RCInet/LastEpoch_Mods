namespace Unity.AssetManager.Core.Editor
{
    /// <summary>
    /// Class used to hold the different storage values from the data provider. Values can be unknown
    /// if the data provider doesn't have them.
    /// </summary>
    class StorageUsage
    {
        public StorageUsage()
        {
            IsUsageBytesKnown = false;
            IsTotalStorageQuotaBytesKnown = false;
        }

        public StorageUsage(ulong usageBytes, ulong totalStorageQuotaBytes)
        {
            IsUsageBytesKnown = true;
            UsageBytes = usageBytes;
            IsTotalStorageQuotaBytesKnown = true;
            TotalStorageQuotaBytes = totalStorageQuotaBytes;
        }

        public bool IsUsageBytesKnown { get; }
        public ulong UsageBytes { get; }

        public bool IsTotalStorageQuotaBytesKnown { get;  }
        public ulong TotalStorageQuotaBytes { get; }
    }
}
