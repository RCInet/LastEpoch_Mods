using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AssetsEmbedded
{
    readonly struct TransformationDescriptor
    {
        /// <summary>
        /// The transformation's ID.
        /// </summary>
        public readonly TransformationId TransformationId;

        /// <summary>
        /// The transformation's dataset descriptor.
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

        /// <inheritdoc cref="CommonEmbedded.DatasetId"/>
        public DatasetId DatasetId => DatasetDescriptor.DatasetId;

        /// <summary>
        /// Creates an instance of the <see cref="TransformationDescriptor"/> struct.
        /// </summary>
        /// <param name="datasetDescriptor">The transformation's dataset descriptor.</param>
        /// <param name="transformationId">The transformation's ID.</param>
        public TransformationDescriptor(DatasetDescriptor datasetDescriptor, TransformationId transformationId)
        {
            TransformationId = transformationId;
            DatasetDescriptor = datasetDescriptor;
        }

         /// <summary>
        /// Returns whether two <see cref="TransformationDescriptor"/> objects are equals.
        /// </summary>
        /// <param name="other">Compare the values with this instance.</param>
        /// <returns>
        /// <see langword="true"/> if both instance have the same values;
        /// <see langword="false"/> otherwise.
        /// </returns>
        public bool Equals(TransformationDescriptor other)
        {
            return DatasetDescriptor.Equals(other.DatasetDescriptor) &&
                   TransformationId.Equals(other.TransformationId);
        }

        /// <summary>
        /// Validate <paramref name="obj"/> is a <see cref="TransformationDescriptor"/> instance and have the same values as this instance.
        /// </summary>
        /// <param name="obj">Compare the values with this instance.</param>
        /// <returns>
        /// <see langword="true"/> if both instance have the same values;
        /// <see langword="false"/> otherwise.
        /// </returns>
        public override bool Equals(object obj) => obj is TransformationDescriptor other && Equals(other);

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
                var hashCode = TransformationId.GetHashCode();
                hashCode = (hashCode * 397) ^ DatasetDescriptor.GetHashCode();
                return hashCode;
            }
        }

        /// <summary>
        /// Checks whether two <see cref="TransformationDescriptor"/> are equal.
        /// </summary>
        /// <param name="left">Compare with this first instance.</param>
        /// <param name="right">Compare with this other instance.</param>
        /// <returns>
        /// <see langword="true"/> if both instances represent the same;
        /// <see langword="false"/> otherwise.
        /// </returns>
        public static bool operator ==(TransformationDescriptor left, TransformationDescriptor right) => left.Equals(right);

        /// <summary>
        /// Checks whether two <see cref="TransformationDescriptor"/> aren't equal.
        /// </summary>
        /// <param name="left">Compare with this first instance.</param>
        /// <param name="right">Compare with this other instance.</param>
        /// <returns>
        /// <see langword="true"/> if both instances are not the same;
        /// <see langword="false"/> if both instances are the same.
        /// </returns>
        public static bool operator !=(TransformationDescriptor left, TransformationDescriptor right) => !left.Equals(right);
    }
}
