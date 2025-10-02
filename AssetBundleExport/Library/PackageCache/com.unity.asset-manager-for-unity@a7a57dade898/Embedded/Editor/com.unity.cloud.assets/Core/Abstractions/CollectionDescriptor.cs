using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AssetsEmbedded
{
    /// <summary>
    /// This struct contains the identifiers for a collection.
    /// </summary>
    readonly struct CollectionDescriptor
    {
        /// <summary>
        /// The asset's project descriptor.
        /// </summary>
        public readonly ProjectDescriptor ProjectDescriptor;

        /// <inheritdoc cref="ProjectDescriptor.OrganizationId"/>
        public OrganizationId OrganizationId => ProjectDescriptor.OrganizationId;

        /// <inheritdoc cref="ProjectDescriptor.ProjectId"/>
        public ProjectId ProjectId => ProjectDescriptor.ProjectId;

        /// <summary>
        /// The path to the collection.
        /// </summary>
        public readonly CollectionPath Path;

        /// <summary>
        /// Creates an instance of the <see cref="CollectionDescriptor"/> struct.
        /// </summary>
        /// <param name="projectDescriptor">The descriptor of the project.</param>
        /// <param name="path">The path to the collection.</param>
        public CollectionDescriptor(ProjectDescriptor projectDescriptor, CollectionPath path)
        {
            ProjectDescriptor = projectDescriptor;
            Path = path;
        }

        /// <summary>
        /// Checks whether two <see cref="CollectionDescriptor"/> objects are equals.
        /// </summary>
        /// <param name="other">Compare the values with this instance.</param>
        /// <returns>
        /// <see langword="true"/> if both instance have the same values;
        /// <see langword="false"/> otherwise.
        /// </returns>
        public bool Equals(CollectionDescriptor other)
        {
            return ProjectDescriptor.Equals(other.ProjectDescriptor) &&
                Path.Equals(other.Path);
        }

        /// <summary>
        /// Validates whether <paramref name="obj"/> is a <see cref="CollectionDescriptor"/> instance and has the same values as this instance.
        /// </summary>
        /// <param name="obj">Compare the values with this instance.</param>
        /// <returns>
        /// <see langword="true"/> if both instance have the same values;
        /// <see langword="false"/> otherwise.
        /// </returns>
        public override bool Equals(object obj) => obj is CollectionDescriptor other && Equals(other);

        /// <summary>
        /// Computes a hash code for the object.
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
                var hashCode = Path.GetHashCode();
                hashCode = (hashCode * 397) ^ ProjectDescriptor.GetHashCode();
                return hashCode;
            }
        }

        /// <summary>
        /// Checks whether two <see cref="CollectionDescriptor"/> are the same.
        /// </summary>
        /// <param name="left">Compare with this first instance.</param>
        /// <param name="right">Compare with this other instance.</param>
        /// <returns>
        /// <see langword="true"/> if both instances represent the same;
        /// <see langword="false"/> otherwise.
        /// </returns>
        public static bool operator ==(CollectionDescriptor left, CollectionDescriptor right) => left.Equals(right);

        /// <summary>
        /// Checks whether two <see cref="CollectionDescriptor"/> are not the same.
        /// </summary>
        /// <param name="left">Compare with this first instance.</param>
        /// <param name="right">Compare with this other instance.</param>
        /// <returns>
        /// <see langword="true"/> if both instances are not the same;
        /// <see langword="false"/> if both instances are the same.
        /// </returns>
        public static bool operator !=(CollectionDescriptor left, CollectionDescriptor right) => !left.Equals(right);

        /// <summary>
        /// Serializes the <see cref="CollectionDescriptor"/> into a JSON string.
        /// </summary>
        /// <returns>A <see cref="CollectionDescriptor"/> serialized as a JSON string. </returns>
        public string ToJson()
        {
            return JsonSerialization.Serialize(new CollectionDescriptorDto
            {
                ProjectDescriptor = ProjectDescriptor.ToJson(),
                CollectionPath = Path.ToString()
            });
        }

        /// <summary>
        /// Deserializes the given JSON string into a <see cref="CollectionDescriptor"/> object.
        /// </summary>
        /// <param name="json">A <see cref="CollectionDescriptor"/> serialized as a JSON string. </param>
        /// <returns>A <see cref="CollectionDescriptor"/>. </returns>
        public static CollectionDescriptor FromJson(string json)
        {
            var dto = JsonSerialization.Deserialize<CollectionDescriptorDto>(json);
            return new CollectionDescriptor(
                ProjectDescriptor.FromJson(dto.ProjectDescriptor),
                new CollectionPath(dto.CollectionPath));
        }
    }
}
