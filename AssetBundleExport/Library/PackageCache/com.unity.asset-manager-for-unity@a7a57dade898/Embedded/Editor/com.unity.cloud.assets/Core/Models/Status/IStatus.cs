namespace Unity.Cloud.AssetsEmbedded
{
    interface IStatus
    {
        /// <summary>
        /// The descriptor of the status flow which owns the status.
        /// </summary>
        StatusFlowDescriptor Descriptor { get; }

        /// <summary>
        /// The id of the status.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// The name of the status.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The description of the status.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Whether the status can be skipped.
        /// </summary>
        bool CanBeSkipped { get; }

        /// <summary>
        /// The order in which the status should be displayed.
        /// </summary>
        int SortingOrder { get; }

        /// <summary>
        /// ???
        /// </summary>
        // StatusPredicate InPredicate { get; }

        /// <summary>
        /// ???
        /// </summary>
        // StatusPredicate OutPredicate { get; }
    }
}
