using System;

namespace Unity.Cloud.AppLinkingEmbedded
{
    /// <summary>
    /// This struct holds information about a process identifier.
    /// </summary>
    readonly struct ProcessId
    {
        readonly string m_String;

        /// <summary>
        /// Returns the value of an identifier representing an invalid process id
        /// </summary>
        public static readonly ProcessId None = new(string.Empty);

        /// <summary>
        /// Creates a <see cref="ProcessId"/> using a <see cref="string"/>.
        /// </summary>
        /// <param name="value">The string representing the process identifier</param>
        public ProcessId(string value) => m_String = value;

        /// <summary>
        /// Returns whether two <see cref="ProcessId"/> objects are equals.
        /// </summary>
        /// <param name="other"></param>
        /// <returns>
        /// <see langword="true"/> if both instances have the same values;
        /// <see langword="false"/> otherwise.
        /// </returns>
        public bool Equals(ProcessId other) => m_String == other.m_String;

        /// <summary>
        /// Validates that  <paramref name="obj"/> is a <see cref="ProcessId"/> instance and that it has the same value as this instance.
        /// </summary>
        /// <param name="obj">Compare the values with this instance.</param>
        /// <returns>
        /// <see langword="true"/> if both instances have the same values;
        /// <see langword="false"/> otherwise.
        /// </returns>
        public override bool Equals(object obj) => obj is ProcessId other && Equals(other);

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
        /// Get the string representation of this <see cref="ProcessId"/>.
        /// </summary>
        /// <returns>The string result.</returns>
        public override string ToString() => m_String;

        /// <summary>
        /// Returns whether two <see cref="ProcessId"/> instances are equal.
        /// </summary>
        /// <param name="left">First instance.</param>
        /// <param name="right">Second instance.</param>
        /// <returns>
        /// <see langword="true"/> if the instances are equal;
        /// <see langword="false"/> otherwise.
        /// </returns>
        public static bool operator ==(ProcessId left, ProcessId right) => left.Equals(right);

        /// <summary>
        /// Returns whether two <see cref="ProcessId"/> instances are different.
        /// </summary>
        /// <param name="left">First instance.</param>
        /// <param name="right">Second instance.</param>
        /// <returns>
        /// <see langword="true"/> if the instances are different;
        /// <see langword="false"/> otherwise.
        /// </returns>
        public static bool operator !=(ProcessId left, ProcessId right) => !left.Equals(right);

        /// <summary>
        /// Explicitly cast a <see cref="ProcessId"/> to a <see cref="string"/>
        /// </summary>
        /// <param name="pId">Object to cast</param>
        /// <returns>The resulting <see cref="string"/></returns>
        public static explicit operator string(ProcessId pId) => pId.m_String;
    }
}
