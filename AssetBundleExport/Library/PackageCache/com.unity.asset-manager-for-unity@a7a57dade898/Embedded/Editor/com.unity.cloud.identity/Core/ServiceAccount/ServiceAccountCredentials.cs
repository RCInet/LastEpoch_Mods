namespace Unity.Cloud.IdentityEmbedded
{
    /// <summary>
    /// This struct holds information about service account credentials.
    /// The expected format is a string composed of the {key_id}:{secret_key} pair as given by Unity Cloud Service Accounts.
    /// </summary>
    struct ServiceAccountCredentials
    {
        readonly string m_String;

        /// <summary>
        /// Returns the value of an identifier representing an invalid service account credentials
        /// </summary>
        public static readonly ServiceAccountCredentials None = new(string.Empty);

        /// <summary>
        /// Creates a <see cref="ServiceAccountCredentials"/> using a <see cref="string"/>.
        /// </summary>
        /// <param name="value">The string representing the service account credentials</param>
        public ServiceAccountCredentials(string value) => m_String = value ?? string.Empty;

        /// <summary>
        /// Returns whether two <see cref="ServiceAccountCredentials"/> objects are equals.
        /// </summary>
        /// <param name="other"></param>
        /// <returns>
        /// <see langword="true"/> if both instances have the same values;
        /// <see langword="false"/> otherwise.
        /// </returns>
        public bool Equals(ServiceAccountCredentials other) => m_String == other.m_String;

        /// <summary>
        /// Validates that  <paramref name="obj"/> is a <see cref="ServiceAccountCredentials"/> instance and that it has the same value as this instance.
        /// </summary>
        /// <param name="obj">Compare the values with this instance.</param>
        /// <returns>
        /// <see langword="true"/> if both instances have the same values;
        /// <see langword="false"/> otherwise.
        /// </returns>
        public override bool Equals(object obj) => obj is ServiceAccountCredentials other && Equals(other);

        /// <summary>
        /// Computes a hash code for the object.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        /// <remarks>
        /// * You should not assume that equal hash codes imply object equality.
        /// * You should never persist or use a hash code outside the application domain in which it was created,
        ///   because the same object may hash differently across application domains, processes, and platforms.
        /// </remarks>
        public override int GetHashCode() => m_String != null ? m_String.GetHashCode() : 0;

        /// <summary>
        /// Get the string representation of this <see cref="ServiceAccountCredentials"/>.
        /// </summary>
        /// <returns>The string result.</returns>
        public override string ToString() => m_String;

        public string ToBase64String() =>
            System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(m_String));

        /// <summary>
        /// Returns whether two <see cref="ServiceAccountCredentials"/> instances are equal.
        /// </summary>
        /// <param name="left">First instance.</param>
        /// <param name="right">Second instance.</param>
        /// <returns>
        /// <see langword="true"/> if the instances are equal;
        /// <see langword="false"/> otherwise.
        /// </returns>
        public static bool operator ==(ServiceAccountCredentials left, ServiceAccountCredentials right) => left.Equals(right);

        /// <summary>
        /// Returns whether two <see cref="ServiceAccountCredentials"/> instances are different.
        /// </summary>
        /// <param name="left">First instance.</param>
        /// <param name="right">Second instance.</param>
        /// <returns>
        /// <see langword="true"/> if the instances are different;
        /// <see langword="false"/> otherwise.
        /// </returns>
        public static bool operator !=(ServiceAccountCredentials left, ServiceAccountCredentials right) => !left.Equals(right);

        /// <summary>
        /// Explicitly cast a <see cref="ServiceAccountCredentials"/> to a <see cref="string"/>
        /// </summary>
        /// <param name="serviceAccountCredentials">Object to cast</param>
        /// <returns>The resulting <see cref="string"/></returns>
        public static explicit operator string(ServiceAccountCredentials serviceAccountCredentials) => serviceAccountCredentials.m_String;
    }
}
