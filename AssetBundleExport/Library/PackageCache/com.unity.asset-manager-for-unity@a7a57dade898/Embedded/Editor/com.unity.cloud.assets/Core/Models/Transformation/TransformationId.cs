using System;

namespace Unity.Cloud.AssetsEmbedded
{
    /// <summary>
    /// Holds information about a transformation identifier.
    /// </summary>
    readonly struct TransformationId
    {
        readonly string m_String;

        /// <summary>
        /// Returns the value of an identifier that represents an invalid dataset Id.
        /// </summary>
        public static readonly TransformationId None = new(Guid.Empty);

        /// <summary>
        /// Returns a <see cref="TransformationId"/> using a <see cref="string"/>.
        /// </summary>
        /// <param name="value">A string that represents the Transformation identifier</param>
        public TransformationId(string value) => m_String = value;

        /// <summary>
        /// Returns a <see cref="TransformationId"/> using a <see cref="Guid"/>.
        /// </summary>
        /// <param name="value">The GUID that represents the Transformation identifier</param>
        public TransformationId(Guid value) => m_String = value.ToString();

        /// <summary>
        /// Compares the values of <see cref="TransformationId"/> objects.
        /// </summary>
        /// <param name="other"></param>
        /// <returns>
        /// <see langword="true"/> if both instance have the same values;
        /// <see langword="false"/> otherwise.
        /// </returns>
        public bool Equals(TransformationId other) => m_String == other.m_String;

        /// <summary>
        /// Checks if <paramref name="obj"/> is an <see cref="TransformationId"/> instance, and then compares the `TransformationId` value with the value of this instance.
        /// </summary>
        /// <param name="obj">Compares values with this instance.</param>
        /// <returns>
        /// <see langword="true"/> if both instances have the same values;
        /// <see langword="false"/> otherwise.
        /// </returns>
        public override bool Equals(object obj) => obj is TransformationId other && Equals(other);

        /// <summary>
        /// Computes a hash code for the object.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        /// <remarks>
        /// * Important: Do not assume that equal hash codes imply object equality.
        /// * Important: Never persist or use a hash code outside the application domain in which it was created,
        ///   because the same object may hash differently across application domains, processes, and platforms.
        /// </remarks>
        public override int GetHashCode() => m_String != null ? m_String.GetHashCode() : 0;

        /// <summary>
        /// Gets the string for this <see cref="TransformationId"/>.
        /// </summary>
        /// <returns>The string result.</returns>
        public override string ToString() => m_String;

        /// <summary>
        /// Checks whether two <see cref="TransformationId"/> are equal.
        /// </summary>
        /// <param name="left">Compare with this first instance.</param>
        /// <param name="right">Compare with this other instance.</param>
        /// <returns>
        /// <see langword="true"/> if both instances represent the same;
        /// <see langword="false"/> otherwise.
        /// </returns>
        public static bool operator ==(TransformationId left, TransformationId right) => left.Equals(right);

        /// <summary>
        /// Checks whether two <see cref="TransformationId"/> aren't equal.
        /// </summary>
        /// <param name="left">Compare with this first instance.</param>
        /// <param name="right">Compare with this other instance.</param>
        /// <returns>
        /// <see langword="true"/> if both instances are not the same;
        /// <see langword="false"/> if both instances are the same.
        /// </returns>
        public static bool operator !=(TransformationId left, TransformationId right) => !left.Equals(right);

        /// <summary>
        /// Explicitly casts a <see cref="TransformationId"/> to a <see cref="string"/>.
        /// </summary>
        /// <param name="dId">Object to cast</param>
        /// <returns>The resulting <see cref="string"/></returns>
        public static explicit operator string(TransformationId dId) => dId.m_String;
    }
}
