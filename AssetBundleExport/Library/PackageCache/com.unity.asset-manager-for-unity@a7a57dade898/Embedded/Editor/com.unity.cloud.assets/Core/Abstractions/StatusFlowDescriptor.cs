using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AssetsEmbedded
{
    /// <summary>
    /// This struct contains the identifiers for a status flow.
    /// </summary>
    readonly struct StatusFlowDescriptor
    {
        /// <summary>
        /// The status' organization genesis ID.
        /// </summary>
        public readonly OrganizationId OrganizationId;

        /// <summary>
        /// A unique id for the status. Uniqueness is scoped to the organization.
        /// </summary>
        public readonly string StatusFlowId;

        /// <summary>
        /// Creates an instance of the <see cref="StatusFlowDescriptor"/> struct.
        /// </summary>
        /// <param name="organizationId">The status' organization genesis ID.</param>
        /// <param name="statusFlowId">The unique id of the status.</param>
        public StatusFlowDescriptor(OrganizationId organizationId, string statusFlowId)
        {
            OrganizationId = organizationId;
            StatusFlowId = statusFlowId;
        }

        /// <summary>
        /// Returns whether two <see cref="StatusFlowDescriptor"/> objects are equals.
        /// </summary>
        /// <param name="other">Compare the values with this instance.</param>
        /// <returns>
        /// <see langword="true"/> if both instance have the same values;
        /// <see langword="false"/> otherwise.
        /// </returns>
        public bool Equals(StatusFlowDescriptor other)
        {
            return OrganizationId.Equals(other.OrganizationId) &&
                string.Equals(StatusFlowId, other.StatusFlowId);
        }

        /// <summary>
        /// Validate <paramref name="obj"/> is a <see cref="StatusFlowDescriptor"/> instance and have the same values as this instance.
        /// </summary>
        /// <param name="obj">Compare the values with this instance.</param>
        /// <returns>
        /// <see langword="true"/> if both instance have the same values;
        /// <see langword="false"/> otherwise.
        /// </returns>
        public override bool Equals(object obj) => obj is StatusFlowDescriptor other && Equals(other);

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
                hashCode = (hashCode * 397) ^ StatusFlowId.GetHashCode();
                return hashCode;
            }
        }

        /// <summary>
        /// Get if two <see cref="StatusFlowDescriptor"/> represent the same.
        /// </summary>
        /// <param name="left">Compare with this first instance.</param>
        /// <param name="right">Compare with this other instance.</param>
        /// <returns>
        /// <see langword="true"/> if both instances represent the same;
        /// <see langword="false"/> otherwise.
        /// </returns>
        public static bool operator ==(StatusFlowDescriptor left, StatusFlowDescriptor right) => left.Equals(right);

        /// <summary>
        /// Get if two <see cref="StatusFlowDescriptor"/> does not represent the same.
        /// </summary>
        /// <param name="left">Compare with this first instance.</param>
        /// <param name="right">Compare with this other instance.</param>
        /// <returns>
        /// <see langword="true"/> if both instances are not the same;
        /// <see langword="false"/> if both instances are the same.
        /// </returns>
        public static bool operator !=(StatusFlowDescriptor left, StatusFlowDescriptor right) => !left.Equals(right);
    }
}
