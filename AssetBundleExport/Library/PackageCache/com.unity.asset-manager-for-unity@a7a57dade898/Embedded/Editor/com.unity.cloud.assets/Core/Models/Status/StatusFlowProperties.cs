namespace Unity.Cloud.AssetsEmbedded
{
    struct StatusFlowProperties
    {
        /// <summary>
        /// The name of the status flow.
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// Whether the status flow is a default flow.
        /// </summary>
        public bool IsDefault { get; internal set; }

        /// <summary>
        /// The id of the starting status.
        /// </summary>
        public string StartStatusId { get; internal set; }
    }
}
