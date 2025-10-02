using System;

namespace Unity.Cloud.CommonEmbedded
{
    /// <summary>
    /// This struct holds information about a permission.
    /// </summary>
    readonly struct Permission
    {
        readonly string m_String;

        /// <summary>
        /// Creates a <see cref="Permission"/> using a <see cref="string"/>.
        /// </summary>
        /// <param name="value">The string representing the permission</param>
        public Permission(string value) => m_String = value;

        /// <summary>
        /// Returns whether two <see cref="Permission"/> objects are equals.
        /// </summary>
        /// <param name="other"></param>
        /// <returns>
        /// <see langword="true"/> if both instances have the same values;
        /// <see langword="false"/> otherwise.
        /// </returns>
        public bool Equals(Permission other) => m_String == other.m_String;

        /// <summary>
        /// Validates that  <paramref name="obj"/> is a <see cref="Permission"/> instance and that it has the same value as this instance.
        /// </summary>
        /// <param name="obj">Compare the values with this instance.</param>
        /// <returns>
        /// <see langword="true"/> if both instances have the same values;
        /// <see langword="false"/> otherwise.
        /// </returns>
        public override bool Equals(object obj) => obj is UserId other && Equals(other);

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
        /// Get the string representation of this <see cref="Permission"/>.
        /// </summary>
        /// <returns>The string result.</returns>
        public override string ToString() => m_String;

        /// <summary>
        /// Returns whether two <see cref="Permission"/> instances are equal.
        /// </summary>
        /// <param name="left">First instance.</param>
        /// <param name="right">Second instance.</param>
        /// <returns>
        /// <see langword="true"/> if the instances are equal;
        /// <see langword="false"/> otherwise.
        /// </returns>
        public static bool operator ==(Permission left, Permission right) => left.Equals(right);

        /// <summary>
        /// Returns whether two <see cref="Permission"/> instances are different.
        /// </summary>
        /// <param name="left">First instance.</param>
        /// <param name="right">Second instance.</param>
        /// <returns>
        /// <see langword="true"/> if the instances are different;
        /// <see langword="false"/> otherwise.
        /// </returns>
        public static bool operator !=(Permission left, Permission right) => !left.Equals(right);

        /// <summary>
        /// Explicitly cast a <see cref="Permission"/> to a <see cref="string"/>
        /// </summary>
        /// <param name="permission">Object to cast</param>
        /// <returns>The resulting <see cref="string"/></returns>
        public static explicit operator string(Permission permission) => permission.m_String;
    }
}
