using System;

namespace Unity.Cloud.IdentityEmbedded
{
    /// <summary>
    /// This struct holds information about a client identifier.
    /// </summary>
    readonly struct ClientId
    {
        readonly string m_String;

        /// <summary>
        /// Returns the value of an identifier representing an invalid client id
        /// </summary>
        public static readonly ClientId None = new(string.Empty);

        /// <summary>
        /// Creates a <see cref="ClientId"/> using a <see cref="string"/>.
        /// </summary>
        /// <param name="value">The string representing the client identifier</param>
        public ClientId(string value) => m_String = value;

        /// <summary>
        /// Returns whether two <see cref="ClientId"/> objects are equals.
        /// </summary>
        /// <param name="other"></param>
        /// <returns>
        /// <see langword="true"/> if both instances have the same values;
        /// <see langword="false"/> otherwise.
        /// </returns>
        public bool Equals(ClientId other) => m_String == other.m_String;

        /// <summary>
        /// Validates that  <paramref name="obj"/> is a <see cref="ClientId"/> instance and that it has the same value as this instance.
        /// </summary>
        /// <param name="obj">Compare the values with this instance.</param>
        /// <returns>
        /// <see langword="true"/> if both instances have the same values;
        /// <see langword="false"/> otherwise.
        /// </returns>
        public override bool Equals(object obj) => obj is ClientId other && Equals(other);

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
        /// Get the string representation of this <see cref="ClientId"/>.
        /// </summary>
        /// <returns>The string result.</returns>
        public override string ToString() => m_String;

        /// <summary>
        /// Returns whether two <see cref="ClientId"/> instances are equal.
        /// </summary>
        /// <param name="left">First instance.</param>
        /// <param name="right">Second instance.</param>
        /// <returns>
        /// <see langword="true"/> if the instances are equal;
        /// <see langword="false"/> otherwise.
        /// </returns>
        public static bool operator ==(ClientId left, ClientId right) => left.Equals(right);

        /// <summary>
        /// Returns whether two <see cref="ClientId"/> instances are different.
        /// </summary>
        /// <param name="left">First instance.</param>
        /// <param name="right">Second instance.</param>
        /// <returns>
        /// <see langword="true"/> if the instances are different;
        /// <see langword="false"/> otherwise.
        /// </returns>
        public static bool operator !=(ClientId left, ClientId right) => !left.Equals(right);

        /// <summary>
        /// Explicitly cast a <see cref="ClientId"/> to a <see cref="string"/>
        /// </summary>
        /// <param name="cId">Object to cast</param>
        /// <returns>The resulting <see cref="string"/></returns>
        public static explicit operator string(ClientId cId) => cId.m_String;
    }
}
