using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AssetsEmbedded
{
    /// <summary>
    /// This struct contains the identifiers for a project.
    /// </summary>
    readonly struct FieldDefinitionDescriptor
    {
        /// <summary>
        /// The project's organization genesis ID.
        /// </summary>
        public readonly OrganizationId OrganizationId;

        /// <summary>
        /// A unique name for the field. Uniqueness is scoped to the organization.
        /// </summary>
        public readonly string FieldKey;

        /// <summary>
        /// Creates an instance of the <see cref="FieldDefinitionDescriptor"/> struct.
        /// </summary>
        /// <param name="organizationId">The project's organization genesis ID.</param>
        /// <param name="fieldKey">The key of the field.</param>
        public FieldDefinitionDescriptor(OrganizationId organizationId, string fieldKey)
        {
            OrganizationId = organizationId;
            FieldKey = fieldKey;
        }

        /// <summary>
        /// Returns whether two <see cref="FieldDefinitionDescriptor"/> objects are equals.
        /// </summary>
        /// <param name="other">Compare the values with this instance.</param>
        /// <returns>
        /// <see langword="true"/> if both instance have the same values;
        /// <see langword="false"/> otherwise.
        /// </returns>
        public bool Equals(FieldDefinitionDescriptor other)
        {
            return OrganizationId.Equals(other.OrganizationId) &&
                FieldKey.Equals(other.FieldKey);
        }

        /// <summary>
        /// Validate <paramref name="obj"/> is a <see cref="FieldDefinitionDescriptor"/> instance and have the same values as this instance.
        /// </summary>
        /// <param name="obj">Compare the values with this instance.</param>
        /// <returns>
        /// <see langword="true"/> if both instance have the same values;
        /// <see langword="false"/> otherwise.
        /// </returns>
        public override bool Equals(object obj) => obj is FieldDefinitionDescriptor other && Equals(other);

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
                hashCode = (hashCode * 397) ^ FieldKey.GetHashCode();
                return hashCode;
            }
        }

        /// <summary>
        /// Get if two <see cref="FieldDefinitionDescriptor"/> represent the same.
        /// </summary>
        /// <param name="left">Compare with this first instance.</param>
        /// <param name="right">Compare with this other instance.</param>
        /// <returns>
        /// <see langword="true"/> if both instances represent the same;
        /// <see langword="false"/> otherwise.
        /// </returns>
        public static bool operator ==(FieldDefinitionDescriptor left, FieldDefinitionDescriptor right) => left.Equals(right);

        /// <summary>
        /// Get if two <see cref="FieldDefinitionDescriptor"/> does not represent the same.
        /// </summary>
        /// <param name="left">Compare with this first instance.</param>
        /// <param name="right">Compare with this other instance.</param>
        /// <returns>
        /// <see langword="true"/> if both instances are not the same;
        /// <see langword="false"/> if both instances are the same.
        /// </returns>
        public static bool operator !=(FieldDefinitionDescriptor left, FieldDefinitionDescriptor right) => !left.Equals(right);

        /// <summary>
        /// Serializes the <see cref="FieldDefinitionDescriptor"/> into a JSON string.
        /// </summary>
        /// <returns>A <see cref="FieldDefinitionDescriptor"/> serialized as a JSON string. </returns>
        public string ToJson()
        {
            return JsonSerialization.Serialize(new FieldDefinitionDescriptorDto
            {
                OrganizationId = OrganizationId.ToString(),
                FieldKey = FieldKey
            });
        }

        /// <summary>
        /// Deserializes the given JSON string into a <see cref="FieldDefinitionDescriptor"/> object.
        /// </summary>
        /// <param name="json">A <see cref="FieldDefinitionDescriptor"/> serialized as a JSON string. </param>
        /// <returns>A <see cref="FieldDefinitionDescriptor"/>. </returns>
        public static FieldDefinitionDescriptor FromJson(string json)
        {
            var dto = JsonSerialization.Deserialize<FieldDefinitionDescriptorDto>(json);
            return new FieldDefinitionDescriptor(
                new OrganizationId(dto.OrganizationId),
                dto.FieldKey);
        }
    }
}
