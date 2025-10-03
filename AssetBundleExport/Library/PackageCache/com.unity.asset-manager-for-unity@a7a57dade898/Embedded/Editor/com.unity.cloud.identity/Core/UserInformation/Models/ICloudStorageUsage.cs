namespace Unity.Cloud.IdentityEmbedded
{
    /// <summary>
    /// An interface that provides Cloud Storage information for an <see cref="Organization"/>.
    /// </summary>
    interface ICloudStorageUsage
    {
        /// <summary>
        /// The total usage bytes count.
        /// </summary>
        public ulong UsageBytes { get; }

        /// <summary>
        /// The total storage quota bytes available.
        /// </summary>
        public ulong TotalStorageQuotaBytes { get; }
    }
}
