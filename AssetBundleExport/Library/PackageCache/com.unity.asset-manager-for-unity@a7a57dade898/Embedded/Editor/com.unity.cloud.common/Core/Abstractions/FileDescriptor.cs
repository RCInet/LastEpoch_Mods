namespace Unity.Cloud.CommonEmbedded
{
    /// <summary>
    /// A struct containing the identifiers for a dataset.
    /// </summary>
    readonly struct FileDescriptor
    {
        /// <summary>
        /// The file's dataset descriptor.
        /// </summary>
        public readonly DatasetDescriptor DatasetDescriptor;

        /// <inheritdoc cref="ProjectDescriptor.OrganizationId"/>
        public OrganizationId OrganizationId => DatasetDescriptor.OrganizationId;

        /// <inheritdoc cref="ProjectDescriptor.ProjectId"/>
        public ProjectId ProjectId => DatasetDescriptor.ProjectId;

        /// <inheritdoc cref="AssetDescriptor.AssetId"/>
        public AssetId AssetId => DatasetDescriptor.AssetId;

        /// <inheritdoc cref="AssetDescriptor.AssetVersion"/>
        public AssetVersion AssetVersion => DatasetDescriptor.AssetVersion;

        /// <inheritdoc cref="DatasetDescriptor.DatasetId"/>
        public DatasetId DatasetId => DatasetDescriptor.DatasetId;

        /// <summary>
        /// The path to the file.
        /// </summary>
        public readonly string Path;

        /// <summary>
        /// Creates an instance of the <see cref="FileDescriptor"/> struct.
        /// </summary>
        /// <param name="datasetDescriptor">The file's dataset descriptor.</param>
        /// <param name="filePath">The file's path.</param>
        public FileDescriptor(DatasetDescriptor datasetDescriptor, string filePath)
        {
            DatasetDescriptor = datasetDescriptor;
            Path = filePath;
        }

        internal FileDescriptor(FileDescriptorDto dto)
        {
            DatasetDescriptor = new DatasetDescriptor(dto.DatasetDescriptor);
            Path = dto.FilePath;
        }

        /// <summary>
        /// Returns whether two <see cref="FileDescriptor"/> objects are equals.
        /// </summary>
        /// <param name="other">Compare the values with this instance.</param>
        /// <returns>
        /// <see langword="true"/> if both instance have the same values;
        /// <see langword="false"/> otherwise.
        /// </returns>
        public bool Equals(FileDescriptor other)
        {
            return DatasetDescriptor.Equals(other.DatasetDescriptor) &&
                Path.Equals(other.Path);
        }

        /// <summary>
        /// Validate <paramref name="obj"/> is a <see cref="FileDescriptor"/> instance and have the same values as this instance.
        /// </summary>
        /// <param name="obj">Compare the values with this instance.</param>
        /// <returns>
        /// <see langword="true"/> if both instance have the same values;
        /// <see langword="false"/> otherwise.
        /// </returns>
        public override bool Equals(object obj) => obj is FileDescriptor other && Equals(other);

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
                var hashCode = Path.GetHashCode();
                hashCode = (hashCode * 397) ^ DatasetDescriptor.GetHashCode();
                return hashCode;
            }
        }

        /// <summary>
        /// Checks whether two <see cref="FileDescriptor"/> are equal.
        /// </summary>
        /// <param name="left">Compare with this first instance.</param>
        /// <param name="right">Compare with this other instance.</param>
        /// <returns>
        /// <see langword="true"/> if both instances represent the same;
        /// <see langword="false"/> otherwise.
        /// </returns>
        public static bool operator ==(FileDescriptor left, FileDescriptor right) => left.Equals(right);

        /// <summary>
        /// Checks whether two <see cref="FileDescriptor"/> aren't equal.
        /// </summary>
        /// <param name="left">Compare with this first instance.</param>
        /// <param name="right">Compare with this other instance.</param>
        /// <returns>
        /// <see langword="true"/> if both instances are not the same;
        /// <see langword="false"/> if both instances are the same.
        /// </returns>
        public static bool operator !=(FileDescriptor left, FileDescriptor right) => !left.Equals(right);

        /// <summary>
        /// Serializes the <see cref="FileDescriptor"/> into a JSON string.
        /// </summary>
        /// <returns>A <see cref="FileDescriptor"/> serialized as a JSON string. </returns>
        public string ToJson()
        {
            return JsonSerialization.Serialize(new FileDescriptorDto
            {
                DatasetDescriptor = new DatasetDescriptorDto(DatasetDescriptor),
                FilePath = Path
            });
        }

        /// <summary>
        /// Deserializes the given JSON string into a <see cref="FileDescriptor"/> object.
        /// </summary>
        /// <param name="json">A <see cref="FileDescriptor"/> serialized as a JSON string. </param>
        /// <returns>A <see cref="FileDescriptor"/>. </returns>
        public static FileDescriptor FromJson(string json)
        {
            var dto = JsonSerialization.Deserialize<FileDescriptorDto>(json);
            return new FileDescriptor(dto);
        }
    }
}
