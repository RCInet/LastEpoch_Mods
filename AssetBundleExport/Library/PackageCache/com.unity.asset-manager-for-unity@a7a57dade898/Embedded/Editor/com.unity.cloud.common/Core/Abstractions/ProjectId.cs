using System;

namespace Unity.Cloud.CommonEmbedded
{
    /// <summary>
    /// This struct holds information about project identifier.
    /// </summary>
    readonly struct ProjectId
    {
        readonly string m_String;

        /// <summary>
        /// Return the value of an identifier representing an invalid project id
        /// </summary>
        public static readonly ProjectId None = new(Guid.Empty);

        /// <summary>
        /// Returns a <see cref="ProjectId"/> using a <see cref="string"/>.
        /// </summary>
        /// <param name="value">The string representing the project identifier</param>
        public ProjectId(string value) => m_String = value;

        /// <summary>
        /// Returns a <see cref="ProjectId"/> using a <see cref="Guid"/>.
        /// </summary>
        /// <param name="value">The guid representing the project identifier</param>
        public ProjectId(Guid value) => m_String = value.ToString();

        /// <summary>
        /// Returns whether two <see cref="ProjectId"/> objects are equals.
        /// </summary>
        /// <param name="other"></param>
        /// <returns>
        /// <see langword="true"/> if both instance have the same values;
        /// <see langword="false"/> otherwise.
        /// </returns>
        public bool Equals(ProjectId other) => m_String == other.m_String;

        /// <summary>
        /// Validate <paramref name="obj"/> is a <see cref="ProjectId"/> instance and have the same values as this instance.
        /// </summary>
        /// <param name="obj">Compare the values with this instance.</param>
        /// <returns>
        /// <see langword="true"/> if both instance have the same values;
        /// <see langword="false"/> otherwise.
        /// </returns>
        public override bool Equals(object obj) => obj is ProjectId other && Equals(other);

        /// <summary>
        /// Compute a hash code for the object.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        /// <remarks>
        /// * You should not assume that equal hash codes imply object equality.
        /// * You should never persist or use a hash code outside the application domain in which it was created,
        ///   because the same object may hash differently across application domains, processes, and platforms.
        /// </remarks>
        public override int GetHashCode() => m_String != null ? m_String.GetHashCode() : 0;

        /// <summary>
        /// Get the string representation of this <see cref="ProjectId"/>.
        /// </summary>
        /// <returns>The string result.</returns>
        public override string ToString() => m_String;

        /// <summary>
        /// Get if two <see cref="ProjectId"/> represent the same.
        /// </summary>
        /// <param name="left">Compare with this first instance.</param>
        /// <param name="right">Compare with this other instance.</param>
        /// <returns>
        /// <see langword="true"/> if both instances represent the same;
        /// <see langword="false"/> otherwise.
        /// </returns>
        public static bool operator ==(ProjectId left, ProjectId right) => left.Equals(right);

        /// <summary>
        /// Get if two <see cref="ProjectId"/> does not represent the same.
        /// </summary>
        /// <param name="left">Compare with this first instance.</param>
        /// <param name="right">Compare with this other instance.</param>
        /// <returns>
        /// <see langword="true"/> if both instances are not the same;
        /// <see langword="false"/> if both instances are the same.
        /// </returns>
        public static bool operator !=(ProjectId left, ProjectId right) => !left.Equals(right);

        /// <summary>
        /// Explicitly cast a <see cref="ProjectId"/> to a <see cref="string"/>>
        /// </summary>
        /// <param name="pId">Object to cast</param>
        /// <returns>The resulting <see cref="string"/></returns>
        public static explicit operator string(ProjectId pId) => pId.m_String;
    }
}
