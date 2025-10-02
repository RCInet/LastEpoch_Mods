namespace Unity.Cloud.AssetsEmbedded
{
    /// <summary>
    /// Defines a parameter that can be used to group assets.
    /// </summary>
    readonly struct Groupable
    {
        internal string Value { get; }

        Groupable(string value)
        {
            Value = value;
        }

        public override string ToString() => Value;

        /// <summary>
        /// Returns a new grouping parameter for a <see cref="GroupableField"/>.
        /// </summary>
        /// <param name="field">A <see cref="GroupableField"/>. </param>
        /// <returns>A <see cref="Groupable"/>. </returns>
        public static Groupable From(GroupableField field)
        {
            return new Groupable(field.From());
        }

        /// <summary>
        /// Returns a new grouping parameter for a metadata field.
        /// </summary>
        /// <param name="metadataOwner">The entity to target. </param>
        /// <param name="metadatafieldKey">A key for a metadata field. </param>
        /// <returns>A <see cref="Groupable"/>. </returns>
        public static Groupable FromMetadata(MetadataOwner metadataOwner, string metadatafieldKey)
        {
            return From(metadataOwner.FromMetadata(), metadatafieldKey);
        }

        /// <summary>
        /// Returns a new grouping parameter for a system metadata field.
        /// </summary>
        /// <param name="metadataOwner">The entity to target. </param>
        /// <param name="systemMetadatafieldKey">A key for a system metadata field. </param>
        /// <returns>A <see cref="Groupable"/>. </returns>
        public static Groupable FromSystemMetadata(MetadataOwner metadataOwner, string systemMetadatafieldKey)
        {
            return From(metadataOwner.FromSystemMetadata(), systemMetadatafieldKey);
        }

        static Groupable From(string prefix, string metadatafieldKey)
        {
            return new Groupable($"{prefix}.{metadatafieldKey}");
        }

        /// <summary>
        /// Implicit conversion from <see cref="GroupableField"/> to <see cref="Groupable"/>.
        /// </summary>
        /// <param name="groupableField">A <see cref="GroupableField"/>. </param>
        /// <returns>A <see cref="Groupable"/>. </returns>
        public static implicit operator Groupable(GroupableField groupableField) => From(groupableField);
    }
}
