using System;

namespace Unity.Cloud.CommonEmbedded
{
    /// <summary>
    /// This struct holds information about a role.
    /// </summary>
    readonly struct Role
    {
        readonly string m_String;

        /// <summary>
        /// Returns the value of a role representing a guest
        /// </summary>
        public static readonly Role Guest = new("guest");

        /// <summary>
        /// Returns the value of a role representing a user
        /// </summary>
        public static readonly Role User = new("user");

        /// <summary>
        /// Returns the value of a role representing a manager
        /// </summary>
        public static readonly Role Manager = new("manager");

        /// <summary>
        /// Returns the value of a role representing an owner
        /// </summary>
        public static readonly Role Owner = new("owner");

        /// <summary>
        /// Returns the value of a role representing a project guest
        /// </summary>
        public static readonly Role ProjectGuest = new("project guest");

        /// <summary>
        /// Creates a <see cref="Role"/> using a <see cref="string"/>.
        /// </summary>
        /// <param name="value">The string representing the role</param>
        public Role(string value) => m_String = value;

        /// <summary>
        /// Returns whether two <see cref="Role"/> objects are equals.
        /// </summary>
        /// <param name="other"></param>
        /// <returns>
        /// <see langword="true"/> if both instances have the same values;
        /// <see langword="false"/> otherwise.
        /// </returns>
        public bool Equals(Role other) => m_String == other.m_String;

        /// <summary>
        /// Validates that  <paramref name="obj"/> is a <see cref="Role"/> instance and that it has the same value as this instance.
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
        /// Get the string representation of this <see cref="Role"/>.
        /// </summary>
        /// <returns>The string result.</returns>
        public override string ToString() => m_String;

        /// <summary>
        /// Returns whether two <see cref="Role"/> instances are equal.
        /// </summary>
        /// <param name="left">First instance.</param>
        /// <param name="right">Second instance.</param>
        /// <returns>
        /// <see langword="true"/> if the instances are equal;
        /// <see langword="false"/> otherwise.
        /// </returns>
        public static bool operator ==(Role left, Role right) => left.Equals(right);

        /// <summary>
        /// Returns whether two <see cref="Role"/> instances are different.
        /// </summary>
        /// <param name="left">First instance.</param>
        /// <param name="right">Second instance.</param>
        /// <returns>
        /// <see langword="true"/> if the instances are different;
        /// <see langword="false"/> otherwise.
        /// </returns>
        public static bool operator !=(Role left, Role right) => !left.Equals(right);

        /// <summary>
        /// Explicitly cast a <see cref="Role"/> to a <see cref="string"/>
        /// </summary>
        /// <param name="role">Object to cast</param>
        /// <returns>The resulting <see cref="string"/></returns>
        public static explicit operator string(Role role) => role.m_String;
    }
}
