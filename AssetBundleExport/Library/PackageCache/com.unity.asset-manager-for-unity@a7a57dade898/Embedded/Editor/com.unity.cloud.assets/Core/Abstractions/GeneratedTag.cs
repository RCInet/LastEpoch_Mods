namespace Unity.Cloud.AssetsEmbedded
{
    /// <summary>
    /// A struct containing the information of a generated tag.
    /// </summary>
    readonly struct GeneratedTag
    {
        /// <summary>
        /// The tag that was generated.
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// A confidence value for the generated tag. This value is between 0 and 1, where 1 is the highest confidence.
        /// </summary>
        public float Confidence { get; }

        internal GeneratedTag(string value, float confidence)
        {
            Value = value;
            Confidence = confidence;
        }
    }
}
