// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// From https://github.com/dotnet/runtime/ at commit 06062e79faab44195f56cd7e4079b22d2380aedd
#nullable enable
using System;

namespace Unity.Cloud.AssetsEmbedded
{
    /// <summary>
    /// Contains some common metadata name-type pairs and helper method to create a metadata name.
    /// </summary>
    internal static class MetadataName
    {
        /// <summary>
        /// Metadata put on a failed lease acquisition to specify when to retry acquiring a lease.
        /// For example, used in <see cref="TokenBucketRateLimiter"/> which periodically replenishes leases.
        /// </summary>
        public static MetadataName<TimeSpan> RetryAfter { get; } = Create<TimeSpan>("RETRY_AFTER");

        /// <summary>
        /// Metadata put on a failed lease acquisition to specify the reason the lease failed.
        /// </summary>
        public static MetadataName<string> ReasonPhrase { get; } = Create<string>("REASON_PHRASE");

        /// <summary>
        /// Create a strongly-typed metadata name.
        /// </summary>
        /// <typeparam name="T">Type that the metadata will contain.</typeparam>
        /// <param name="name">Name of the metadata.</param>
        /// <returns></returns>
        public static MetadataName<T> Create<T>(string name) => new MetadataName<T>(name);
    }

    /// <summary>
    /// A strongly-typed name of metadata that can be stored in a <see cref="RateLimitLease"/>.
    /// </summary>
    /// <typeparam name="T">The type the metadata will be.</typeparam>
    internal class MetadataName<T> : IEquatable<MetadataName<T>>
    {
        private readonly string m_name;

        /// <summary>
        /// Constructs a <see cref="MetadataName{T}"/> object with the given name.
        /// </summary>
        /// <param name="name">The name of the <see cref="MetadataName"/> object.</param>
        public MetadataName(string name)
        {
            m_name = name ?? throw new ArgumentNullException(nameof(name));
        }

        /// <summary>
        /// Gets the name of the metadata.
        /// </summary>
        public string Name => m_name;

        /// <inheritdoc/>
        public override string ToString()
        {
            return m_name;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return m_name.GetHashCode();
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            return obj is MetadataName<T> m && Equals(m);
        }

        /// <inheritdoc/>
        public bool Equals(MetadataName<T>? other)
        {
            if (other is null)
            {
                return false;
            }

            return string.Equals(m_name, other.m_name, StringComparison.Ordinal);
        }

        /// <summary>
        /// Determines whether two <see cref="MetadataName{T}"/> are equal to each other.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(MetadataName<T> left, MetadataName<T> right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Determines whether two <see cref="MetadataName{T}"/> are not equal to each other.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(MetadataName<T> left, MetadataName<T> right)
        {
            return !(left == right);
        }
    }
}
