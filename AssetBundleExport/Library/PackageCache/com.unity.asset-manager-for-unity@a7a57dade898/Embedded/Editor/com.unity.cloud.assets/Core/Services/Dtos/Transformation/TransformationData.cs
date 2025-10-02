using System;
using System.Collections.Generic;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AssetsEmbedded
{
    class TransformationData : ITransformationData
    {
        /// <inheritdoc/>
        public TransformationId Id { get; set; }

        public UserId UserId { get; set; }

        /// <inheritdoc/>
        public AssetId AssetId { get; set; }

        /// <inheritdoc/>
        public AssetVersion AssetVersion { get; set; }

        /// <inheritdoc/>
        public DatasetId InputDatasetId { get; set; }

        /// <inheritdoc/>
        public DatasetId OutputDatasetId { get; set; }

        /// <inheritdoc/>
        public DatasetId LinkDatasetId { get; set; }

        /// <inheritdoc/>
        public IEnumerable<string> InputFiles { get; set; }

        /// <inheritdoc/>
        public string WorkflowType { get; set; }

        /// <inheritdoc/>
        public TransformationStatus Status { get; set; }

        /// <inheritdoc/>
        public string ErrorMessage { get; set; }

        /// <inheritdoc/>
        public int Progress { get; set; }

        /// <inheritdoc/>
        public DateTime CreatedOn { get; set; }

        /// <inheritdoc/>
        public DateTime UpdatedAt { get; set; }

        /// <inheritdoc/>
        public DateTime StartedAt { get; set; }

        /// <inheritdoc/>
        public string JobId { get; set; }
    }
}
