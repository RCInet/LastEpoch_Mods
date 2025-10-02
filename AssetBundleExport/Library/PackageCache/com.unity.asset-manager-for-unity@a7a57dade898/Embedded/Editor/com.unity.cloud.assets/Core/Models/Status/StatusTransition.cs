using System;

namespace Unity.Cloud.AssetsEmbedded
{
    class StatusTransition : IStatusTransition
    {
        /// <inheritdoc />
        public StatusFlowDescriptor Descriptor { get; }

        public string Id { get; }

        /// <inheritdoc />
        public string FromStatusId { get; set; }

        /// <inheritdoc />
        public string ToStatusId { get; set; }

        /// <inheritdoc />
        public StatusPredicate ThroughPredicate { get; set; }

        internal StatusTransition(StatusFlowDescriptor descriptor, string id)
        {
            Descriptor = descriptor;
            Id = id;
        }
    }
}
