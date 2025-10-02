using System;
using System.Text;

namespace Unity.Cloud.AssetsEmbedded
{
    /// <summary>
    /// Represents a path to an element.
    /// </summary>
    readonly struct CollectionPath : IEquatable<CollectionPath>, IEquatable<string>
    {
        internal const char k_PathDelimiter = '/';

        readonly string m_Path;

        /// <summary>
        /// Returns whether the path is empty.
        /// </summary>
        public bool IsEmpty => string.IsNullOrWhiteSpace(m_Path);

        /// <summary>
        /// Returns the length of the path.
        /// </summary>
        public int Length => m_Path.Length;

        /// <summary>
        /// Initializes and returns an instance of <see cref="CollectionPath"/>
        /// </summary>
        /// <param name="path">A path. </param>
        public CollectionPath(string path)
        {
            this.m_Path = string.IsNullOrEmpty(path) ? "" : path;
        }

        /// <summary>
        /// Returns the components of the path.
        /// </summary>
        /// <returns>An array of string representing every element in the path. </returns>
        public string[] GetPathComponents()
        {
            return m_Path.Split(k_PathDelimiter);
        }

        /// <summary>
        /// Checks whether the path contains the given string.
        /// </summary>
        /// <param name="str">A string to verify. </param>
        /// <returns>True if the parameter string is contained within the path. </returns>
        public bool Contains(string str)
        {
            return m_Path.Contains(str);
        }

        /// <summary>
        /// Checks whether the path starts with the given string.
        /// </summary>
        /// <param name="str">A string to verify. </param>
        /// <returns>True if the parameter string matches the beginning of the path. </returns>
        public bool StartsWith(string str)
        {
            return m_Path.StartsWith(str);
        }

        /// <summary>
        /// Checks whether the path ends with the given string.
        /// </summary>
        /// <param name="str">A string to verify. </param>
        /// <returns>True if the parameter string matches the end of the path. </returns>
        public bool EndsWith(string str)
        {
            return m_Path.EndsWith(str);
        }

        public override bool Equals(object obj)
        {
            return obj is CollectionPath other && Equals(other);
        }

        public bool Equals(string str)
        {
            return m_Path.Equals(str);
        }

        public bool Equals(CollectionPath other)
        {
            return Equals(other.m_Path);
        }

        public override int GetHashCode()
        {
            return m_Path != null ? m_Path.GetHashCode() : 0;
        }

        public override string ToString()
        {
            return m_Path;
        }

        public static bool operator ==(CollectionPath a, CollectionPath b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(CollectionPath a, CollectionPath b)
        {
            return !(a == b);
        }

        public static implicit operator string(CollectionPath a) => a.m_Path;

        public static implicit operator CollectionPath(string a) => new(a);

        /// <summary>
        /// Creates a new path from the given paths.
        /// </summary>
        /// <param name="startPath">The beginning of the new path. </param>
        /// <param name="relativePath">The end of the new path. </param>
        /// <returns>A new <see cref="CollectionPath"/> that is the combination of the inputs. </returns>
        public static CollectionPath CombinePaths(CollectionPath startPath, CollectionPath relativePath)
        {
            if (startPath.IsEmpty)
            {
                return relativePath;
            }

            return relativePath.IsEmpty ? startPath : new CollectionPath(string.Join(k_PathDelimiter, startPath.m_Path, relativePath.m_Path));
        }

        /// <summary>
        /// Creates a string path in the format of a <see cref="CollectionPath"/> from the given inputs.
        /// </summary>
        /// <param name="components">The individual elements of the path. </param>
        /// <returns>A string path combining all inputs.</returns>
        public static string BuildPath(params string[] components)
        {
            var strBuilder = new StringBuilder();
            strBuilder.AppendJoin(k_PathDelimiter, components);

            return strBuilder.ToString();
        }
    }

    static class CollectionPathUtilities
    {
        /// <summary>
        /// Trims the <paramref name="path"/> back to the specified parent.
        /// </summary>
        /// <param name="path">The path to query. </param>
        /// <param name="trimCount">The number of components to trim from the path. </param>
        /// <returns>A path to a parent. </returns>
        public static CollectionPath GetParentPath(this CollectionPath path, int trimCount = 1)
        {
            var components = path.GetPathComponents();
            if (components.Length - trimCount > 0)
            {
                var strBuilder = new StringBuilder(components[0]);
                for (var i = 1; i < components.Length - trimCount; ++i)
                {
                    strBuilder.Append(CollectionPath.k_PathDelimiter);
                    strBuilder.Append(components[i]);
                }

                return strBuilder.ToString();
            }

            return new CollectionPath("");
        }

        /// <summary>
        /// Returns the last component of the path.
        /// </summary>
        /// <param name="path">The path to query. </param>
        /// <returns>The . </returns>
        public static string GetLastComponentOfPath(this CollectionPath path)
        {
            var components = path.GetPathComponents();
            return components[^1];
        }
    }
}
