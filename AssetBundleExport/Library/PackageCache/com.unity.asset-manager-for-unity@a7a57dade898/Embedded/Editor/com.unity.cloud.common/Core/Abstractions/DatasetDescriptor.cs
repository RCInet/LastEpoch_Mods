
namespace Unity.Cloud.CommonEmbedded
{
    /// <summary>
    /// A struct containing the identifiers for a dataset.
    /// </summary>
    readonly struct DatasetDescriptor
    {
        /// <summary>
        /// The dataset's asset descriptor.
        /// </summary>
        public readonly AssetDescriptor AssetDescriptor;

        /// <inheritdoc cref="ProjectDescriptor.OrganizationId"/>
        public OrganizationId OrganizationId => AssetDescriptor.OrganizationId;

        /// <inheritdoc cref="ProjectDescriptor.ProjectId"/>
        public ProjectId ProjectId => AssetDescriptor.ProjectId;

        /// <inheritdoc cref="AssetDescriptor.AssetId"/>
        public AssetId AssetId => AssetDescriptor.AssetId;

        /// <inheritdoc cref="AssetDescriptor.AssetVersion"/>
        public AssetVersion AssetVersion => AssetDescriptor.AssetVersion;

        /// <summary>
        /// The dataset's ID.
        /// </summary>
        public readonly DatasetId DatasetId;

        /// <summary>
        /// Creates an instance of the <see cref="DatasetDescriptor"/> struct.
        /// </summary>
        /// <param name="assetDescriptor">The dataset's asset descriptor.</param>
        /// <param name="datasetId">The dataset's ID.</param>
        public DatasetDescriptor(AssetDescriptor assetDescriptor, DatasetId datasetId)
        {
            AssetDescriptor = assetDescriptor;
            DatasetId = datasetId;
        }

        internal DatasetDescriptor(DatasetDescriptorDto dto)
        {
            AssetDescriptor = new AssetDescriptor(dto.AssetDescriptor);
            DatasetId = new DatasetId(dto.DatasetId);
        }

        /// <summary>
        /// Returns whether two <see cref="DatasetDescriptor"/> objects are equals.
        /// </summary>
        /// <param name="other">Compare the values with this instance.</param>
        /// <returns>
        /// <see langword="true"/> if both instance have the same values;
        /// <see langword="false"/> otherwise.
        /// </returns>
        public bool Equals(DatasetDescriptor other)
        {
            return AssetDescriptor.Equals(other.AssetDescriptor) &&
                   DatasetId.Equals(other.DatasetId);
        }

        /// <summary>
        /// Validate <paramref name="obj"/> is a <see cref="DatasetDescriptor"/> instance and have the same values as this instance.
        /// </summary>
        /// <param name="obj">Compare the values with this instance.</param>
        /// <returns>
        /// <see langword="true"/> if both instance have the same values;
        /// <see langword="false"/> otherwise.
        /// </returns>
        public override bool Equals(object obj) => obj is DatasetDescriptor other && Equals(other);

        /// <summary>
        /// Compute a hash code for the object.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        /// <remarks>
        /// * You should not assume that equal hash codes imply object equality.
        /// * You should never persist or use a hash code outside the application domain in which it was created,
        ///   because the same object may hash differently across application domains, processes, and platforms.
        /// </remarks>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = DatasetId.GetHashCode();
                hashCode = (hashCode * 397) ^ AssetDescriptor.GetHashCode();
                return hashCode;
            }
        }

        /// <summary>
        /// Checks whether two <see cref="DatasetDescriptor"/> are equal.
        /// </summary>
        /// <param name="left">Compare with this first instance.</param>
        /// <param name="right">Compare with this other instance.</param>
        /// <returns>
        /// <see langword="true"/> if both instances represent the same;
        /// <see langword="false"/> otherwise.
        /// </returns>
        public static bool operator ==(DatasetDescriptor left, DatasetDescriptor right) => left.Equals(right);

        /// <summary>
        /// Checks whether two <see cref="DatasetDescriptor"/> aren't equal.
        /// </summary>
        /// <param name="left">Compare with this first instance.</param>
        /// <param name="right">Compare with this other instance.</param>
        /// <returns>
        /// <see langword="true"/> if both instances are not the same;
        /// <see langword="false"/> if both instances are the same.
        /// </returns>
        public static bool operator !=(DatasetDescriptor left, DatasetDescriptor right) => !left.Equals(right);

        /// <summary>
        /// Serializes the <see cref="DatasetDescriptor"/> into a JSON string.
        /// </summary>
        /// <returns>A <see cref="DatasetDescriptor"/> serialized as a JSON string. </returns>
        public string ToJson()
        {
            return JsonSerialization.Serialize(new DatasetDescriptorDto
            {
                AssetDescriptor = new AssetDescriptorDto(AssetDescriptor),
                DatasetId = DatasetId.ToString()
            });
        }

        /// <summary>
        /// Deserializes the given JSON string into a <see cref="DatasetDescriptor"/> object.
        /// </summary>
        /// <param name="json">A <see cref="DatasetDescriptor"/> serialized as a JSON string. </param>
        /// <returns>A <see cref="DatasetDescriptor"/>. </returns>
        public static DatasetDescriptor FromJson(string json)
        {
            var dto = JsonSerialization.Deserialize<DatasetDescriptorDto>(json);
            return new DatasetDescriptor(dto);
        }
    }
}
