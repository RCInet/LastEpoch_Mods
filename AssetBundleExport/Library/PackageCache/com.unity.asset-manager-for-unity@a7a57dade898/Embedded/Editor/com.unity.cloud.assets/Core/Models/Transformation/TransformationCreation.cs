namespace Unity.Cloud.AssetsEmbedded
{
    class TransformationCreation : ITransformationCreation
    {
        /// <inheritdoc />
        public WorkflowType WorkflowType { get; set; }

        /// <inheritdoc />
        public string CustomWorkflowName { get; set; }

        /// <inheritdoc />
        public string[] InputFilePaths { get; set; }
    }
}
