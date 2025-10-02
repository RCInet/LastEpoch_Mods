namespace Unity.Cloud.IdentityEmbedded
{
    /// <summary>
    /// A class that provides Cloud Storage information for an <see cref="Organization"/>.
    /// </summary>
    class CloudStorageUsage : ICloudStorageUsage
    {
        /// <inheritdoc/>
        public ulong UsageBytes { get; set; }

        /// <inheritdoc/>
        public ulong TotalStorageQuotaBytes { get; set; }

        internal CloudStorageUsage(CloudStorageUsageJson cloudStorageUsageJson)
        {
            UsageBytes = cloudStorageUsageJson.UsageBytes;
            TotalStorageQuotaBytes = cloudStorageUsageJson.TotalStorageQuotaBytes;
        }
    }
}
