using System;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.IdentityEmbedded
{
    /// <summary>
    /// An interface that exposes project information.
    /// </summary>
    interface IProject : IRoleProvider, IMemberInfoProvider
    {
        /// <summary>
        /// Gets the id of the project.
        /// </summary>
        ProjectDescriptor Descriptor { get; }

        /// <summary>
        /// Gets the name of the organization.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the name of the project.
        /// </summary>
        public string IconUrl { get; }

        /// <summary>
        /// Gets the date time of creation the project.
        /// </summary>
        public DateTime? CreatedAt { get; }

        /// <summary>
        /// Gets the date time of last update the project.
        /// </summary>
        public DateTime? UpdatedAt { get; }

        /// <summary>
        /// Gets the date time of last update the project.
        /// </summary>
        public DateTime? ArchivedAt { get; }

        /// <summary>
        /// Gets if the project is enabled in Asset Manager.
        /// </summary>
        public bool EnabledInAssetManager { get; }
    }

    internal class AssetProjectJson
    {
        public string Id { get; set; }
    }

}
