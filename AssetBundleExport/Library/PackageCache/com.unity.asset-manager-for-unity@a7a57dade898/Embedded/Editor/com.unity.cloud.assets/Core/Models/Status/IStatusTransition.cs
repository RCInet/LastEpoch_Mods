namespace Unity.Cloud.AssetsEmbedded
{
    interface IStatusTransition
    {
        /// <summary>
        /// The descriptor of the status flow which owns the transition.
        /// </summary>
        StatusFlowDescriptor Descriptor { get; }

        /// <summary>
        /// The id of the transition.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// The id of the status from which the transition originates.
        /// </summary>
        string FromStatusId { get; }

        /// <summary>
        /// The id of the status to which the transition leads.
        /// </summary>
        string ToStatusId { get; }

        /// <summary>
        /// ???
        /// </summary>
        // StatusPredicate ThroughPredicate { get; }
    }
}
