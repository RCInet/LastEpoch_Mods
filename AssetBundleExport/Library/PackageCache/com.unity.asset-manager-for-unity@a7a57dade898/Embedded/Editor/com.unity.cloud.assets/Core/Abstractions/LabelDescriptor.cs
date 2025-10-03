using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AssetsEmbedded
{
    /// <summary>
    /// This struct contains the identifiers for a label.
    /// </summary>
    readonly struct LabelDescriptor
    {
        /// <summary>
        /// The label's organization genesis ID.
        /// </summary>
        public readonly OrganizationId OrganizationId;

        /// <summary>
        /// A unique name for the label. Uniqueness is scoped to the organization.
        /// </summary>
        public readonly string LabelName;

        /// <summary>
        /// Creates an instance of the <see cref="LabelDescriptor"/> struct.
        /// </summary>
        /// <param name="organizationId">The label's organization genesis ID.</param>
        /// <param name="labelName">The unique name of the label.</param>
        public LabelDescriptor(OrganizationId organizationId, string labelName)
        {
            OrganizationId = organizationId;
            LabelName = labelName;
        }

        /// <summary>
        /// Returns whether two <see cref="LabelDescriptor"/> objects are equals.
        /// </summary>
        /// <param name="other">Compare the values with this instance.</param>
        /// <returns>
        /// <see langword="true"/> if both instance have the same values;
        /// <see langword="false"/> otherwise.
        /// </returns>
        public bool Equals(LabelDescriptor other)
        {
            return OrganizationId.Equals(other.OrganizationId) &&
                LabelName.Equals(other.LabelName);
        }

        /// <summary>
        /// Validate <paramref name="obj"/> is a <see cref="LabelDescriptor"/> instance and have the same values as this instance.
        /// </summary>
        /// <param name="obj">Compare the values with this instance.</param>
        /// <returns>
        /// <see langword="true"/> if both instance have the same values;
        /// <see langword="false"/> otherwise.
        /// </returns>
        public override bool Equals(object obj) => obj is LabelDescriptor other && Equals(other);

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
                var hashCode = OrganizationId.GetHashCode();
                hashCode = (hashCode * 397) ^ LabelName.GetHashCode();
                return hashCode;
            }
        }

        /// <summary>
        /// Get if two <see cref="LabelDescriptor"/> represent the same.
        /// </summary>
        /// <param name="left">Compare with this first instance.</param>
        /// <param name="right">Compare with this other instance.</param>
        /// <returns>
        /// <see langword="true"/> if both instances represent the same;
        /// <see langword="false"/> otherwise.
        /// </returns>
        public static bool operator ==(LabelDescriptor left, LabelDescriptor right) => left.Equals(right);

        /// <summary>
        /// Get if two <see cref="LabelDescriptor"/> does not represent the same.
        /// </summary>
        /// <param name="left">Compare with this first instance.</param>
        /// <param name="right">Compare with this other instance.</param>
        /// <returns>
        /// <see langword="true"/> if both instances are not the same;
        /// <see langword="false"/> if both instances are the same.
        /// </returns>
        public static bool operator !=(LabelDescriptor left, LabelDescriptor right) => !left.Equals(right);
    }
}
