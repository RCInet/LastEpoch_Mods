using System;
using System.Reflection;

namespace Unity.Cloud.CommonEmbedded
{
    /// <summary>
    /// A class containing version information about the source API.
    /// </summary>
    class ApiSourceVersion
    {
        /// <summary>
        /// Default dev version for the API source version.
        /// </summary>
        public const string k_DevVersion = "dev";

        /// <summary>
        /// The name of the source API.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The version of the source API.
        /// </summary>
        public string Version { get; }

        /// <summary>
        /// Creates an instance of the <see cref="ApiSourceVersion"/> with the provided name and version.
        /// </summary>
        /// <param name="name">The API name.</param>
        /// <param name="version">The API version.</param>
        /// <exception cref="ArgumentNullException">Thrown if the <paramref name="name"/> or <paramref name="version"/> are null or empty.</exception>
        public ApiSourceVersion(string name, string version)
        {
            Name = !string.IsNullOrEmpty(name) ? name : throw new ArgumentNullException(nameof(name));
            Version = !string.IsNullOrEmpty(version) ? version : throw new ArgumentNullException(nameof(version));
        }

        /// <summary>
        /// Returns the <see cref="ApiSourceVersion"/> for the calling assembly.
        /// The source values are retrieved from the <see cref="ApiSourceVersionAttribute"/> which must be defined in the calling <see cref="Assembly"/>.
        /// </summary>
        /// <remarks>An instance of the <see cref="ApiSourceVersionAttribute"/> must be defined at the assembly-level in the calling <see cref="Assembly"/> in order
        /// for the correct API source values to be added as a header.</remarks>
        /// <param name="assembly">The target assembly.</param>
        /// <returns>The retrieved <see cref="ApiSourceVersion"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the <paramref name="assembly"/> is null or if the name or version defined in the retrieved <see cref="ApiSourceVersionAttribute"/> are null or white space.</exception>
        /// <exception cref="InvalidOperationException">Thrown if <see cref="ApiSourceVersionAttribute"/> does not exist or is not initialized in the calling assembly.</exception>
        /// <exception cref="InvalidArgumentException">Thrown if <see cref="ApiSourceVersionAttribute"/> is initialized with null or empty values in the calling assembly.</exception>
        public static ApiSourceVersion GetApiSourceVersionForAssembly(Assembly assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly));

            // Retrieve the API source information for the calling assembly.
            var apiSourceVersionAttribute = assembly.GetCustomAttribute<ApiSourceVersionAttribute>();

            if (apiSourceVersionAttribute == null)
                throw new InvalidOperationException($"{nameof(ApiSourceVersionAttribute)} does not exist or is not initialized in the calling assembly: {assembly.GetName().Name}.");

            // Ensure all required API source values are set.
            var apiSourceVersion = apiSourceVersionAttribute.apiSourceVersion;
            if (apiSourceVersion == null || string.IsNullOrWhiteSpace(apiSourceVersion.Name) || string.IsNullOrWhiteSpace(apiSourceVersion.Version))
                throw new InvalidArgumentException($"All values in {nameof(ApiSourceVersionAttribute)} must be set.");

            return apiSourceVersion;
        }
    }

    /// <summary>
    /// An attribute to store <see cref="apiSourceVersion"/> for a given <see cref="System.Reflection.Assembly"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly)]
sealed class ApiSourceVersionAttribute : Attribute
    {
        /// <summary>
        /// Returns the stored <see cref="apiSourceVersion"/>.
        /// </summary>
        public ApiSourceVersion apiSourceVersion { get; }

        /// <summary>
        /// Creates an instance of the <see cref="ApiSourceVersionAttribute"/> with the provided name and version.
        /// </summary>
        /// <param name="name">The API name.</param>
        /// <param name="version">The API version.</param>
        /// <exception cref="ArgumentNullException">Thrown if the <paramref name="name"/> or <paramref name="version"/> are null or empty.</exception>
        public ApiSourceVersionAttribute(string name, string version)
        {
            apiSourceVersion = new ApiSourceVersion(name, version);
        }
    }
}
