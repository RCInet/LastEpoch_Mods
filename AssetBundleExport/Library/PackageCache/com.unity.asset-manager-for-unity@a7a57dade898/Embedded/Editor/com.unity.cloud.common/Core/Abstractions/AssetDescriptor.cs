
namespace Unity.Cloud.CommonEmbedded
{
    /// <summary>
    /// A struct containing the identifiers for an asset.
    /// </summary>
    readonly struct AssetDescriptor
    {
        /// <summary>
        /// The asset's project descriptor.
        /// </summary>
        public readonly ProjectDescriptor ProjectDescriptor;

        /// <inheritdoc cref="CommonEmbedded.ProjectDescriptor.OrganizationId"/>
        public OrganizationId OrganizationId => ProjectDescriptor.OrganizationId;

        /// <inheritdoc cref="ProjectDescriptor.ProjectId"/>
        public ProjectId ProjectId => ProjectDescriptor.ProjectId;

        /// <summary>
        /// The asset's ID.
        /// </summary>
        public readonly AssetId AssetId;

        /// <summary>
        /// The asset's version.
        /// </summary>
        public readonly AssetVersion AssetVersion;

        /// <summary>
        /// Creates an instance of the <see cref="AssetDescriptor"/> struct.
        /// </summary>
        /// <param name="projectDescriptor">The asset's project descriptor.</param>
        /// <param name="assetId">The asset's ID</param>
        /// <param name="assetVersion">The asset's version.</param>
        public AssetDescriptor(ProjectDescriptor projectDescriptor, AssetId assetId, AssetVersion assetVersion)
        {
            ProjectDescriptor = projectDescriptor;
            AssetId = assetId;
            AssetVersion = assetVersion;
        }

        internal AssetDescriptor(AssetDescriptorDto dto)
        {
            ProjectDescriptor = new ProjectDescriptor(dto.ProjectDescriptor);
            AssetId = new AssetId(dto.AssetId);
            AssetVersion = new AssetVersion(dto.AssetVersion);
        }

        /// <summary>
        /// Returns whether two <see cref="AssetDescriptor"/> objects are equals.
        /// </summary>
        /// <param name="other">Compare the values with this instance.</param>
        /// <returns>
        /// <see langword="true"/> if both instance have the same values;
        /// <see langword="false"/> otherwise.
        /// </returns>
        public bool Equals(AssetDescriptor other)
        {
            return ProjectDescriptor.Equals(other.ProjectDescriptor) &&
                AssetId.Equals(other.AssetId) &&
                AssetVersion.Equals(other.AssetVersion);
        }

        /// <summary>
        /// Validate <paramref name="obj"/> is a <see cref="AssetDescriptor"/> instance and have the same values as this instance.
        /// </summary>
        /// <param name="obj">Compare the values with this instance.</param>
        /// <returns>
        /// <see langword="true"/> if both instance have the same values;
        /// <see langword="false"/> otherwise.
        /// </returns>
        public override bool Equals(object obj) => obj is AssetDescriptor other && Equals(other);

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
                var hashCode = AssetId.GetHashCode();
                hashCode = (hashCode * 397) ^ ProjectDescriptor.GetHashCode();
                hashCode = (hashCode * 397) ^ AssetVersion.GetHashCode();
                return hashCode;
            }
        }

        /// <summary>
        /// Get if two <see cref="AssetDescriptor"/> represent the same.
        /// </summary>
        /// <param name="left">Compare with this first instance.</param>
        /// <param name="right">Compare with this other instance.</param>
        /// <returns>
        /// <see langword="true"/> if both instances represent the same;
        /// <see langword="false"/> otherwise.
        /// </returns>
        public static bool operator ==(AssetDescriptor left, AssetDescriptor right) => left.Equals(right);

        /// <summary>
        /// Get if two <see cref="AssetDescriptor"/> does not represent the same.
        /// </summary>
        /// <param name="left">Compare with this first instance.</param>
        /// <param name="right">Compare with this other instance.</param>
        /// <returns>
        /// <see langword="true"/> if both instances are not the same;
        /// <see langword="false"/> if both instances are the same.
        /// </returns>
        public static bool operator !=(AssetDescriptor left, AssetDescriptor right) => !left.Equals(right);

        /// <summary>
        /// Serializes the <see cref="AssetDescriptor"/> into a JSON string.
        /// </summary>
        /// <returns>A <see cref="AssetDescriptor"/> serialized as a JSON string. </returns>
        public string ToJson()
        {
            return JsonSerialization.Serialize(new AssetDescriptorDto
            {
                ProjectDescriptor = new ProjectDescriptorDto(ProjectDescriptor),
                AssetId = AssetId.ToString(),
                AssetVersion = AssetVersion.ToString()
            });
        }

        /// <summary>
        /// Deserializes the given JSON string into a <see cref="AssetDescriptor"/> object.
        /// </summary>
        /// <param name="json">A <see cref="AssetDescriptor"/> serialized as a JSON string. </param>
        /// <returns>A <see cref="AssetDescriptor"/>. </returns>
        public static AssetDescriptor FromJson(string json)
        {
            var dto = JsonSerialization.Deserialize<AssetDescriptorDto>(json);
            return new AssetDescriptor(dto);
        }
    }
}
