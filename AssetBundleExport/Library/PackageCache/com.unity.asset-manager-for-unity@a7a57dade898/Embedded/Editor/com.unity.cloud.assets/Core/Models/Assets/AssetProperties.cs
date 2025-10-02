using System;
using System.Collections.Generic;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AssetsEmbedded
{
    /// <summary>
    /// The properties of an <see cref="IAsset"/>.
    /// </summary>
    struct AssetProperties
    {
        /// <summary>
        /// The frozen state of the asset.
        /// </summary>
        public AssetState State { get; internal set; }

        /// <summary>
        /// The sequence number of the asset. This will only be populated if the version is frozen.
        /// </summary>
        public int FrozenSequenceNumber { get; internal set; }

        /// <summary>
        /// The change log of the asset version.
        /// </summary>
        public string Changelog { get; internal set; }

        /// <summary>
        /// The version id from which this version was branched.
        /// </summary>
        public AssetVersion ParentVersion { get; internal set; }

        /// <summary>
        /// The sequence number from which this version was branched.
        /// </summary>
        public int ParentFrozenSequenceNumber { get; internal set; }

        /// <summary>
        /// The source project of the asset.
        /// </summary>
        public ProjectDescriptor SourceProject { get; internal set; }

        /// <summary>
        /// The list of projects the asset is linked to.
        /// </summary>
        public IEnumerable<ProjectDescriptor> LinkedProjects { get; internal set; }

        /// <summary>
        /// The name of the asset.
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// The description of the asset.
        /// </summary>
        public string Description { get; internal set; }

        /// <summary>
        /// The type of the asset.
        /// </summary>
        public AssetType Type { get; internal set; }

        /// <summary>
        /// The tags of the asset.
        /// </summary>
        public IEnumerable<string> Tags { get; internal set; }

        /// <summary>
        /// The tags of the asset.
        /// </summary>
        public IEnumerable<string> SystemTags { get; internal set; }

        /// <summary>
        /// The labels associated to the asset version.
        /// </summary>
        public IEnumerable<LabelDescriptor> Labels { get; internal set; }

        /// <summary>
        /// The labels no longer associated to the asset version.
        /// </summary>
        public IEnumerable<LabelDescriptor> ArchivedLabels { get; internal set; }

        /// <summary>
        /// The descriptor for the preview file of the asset.
        /// </summary>
        public FileDescriptor? PreviewFileDescriptor { get; internal set; }

        /// <summary>
        /// The status name of the asset as identified by <see cref="IStatus.Name"/>.
        /// </summary>
        public string StatusName { get; internal set; }

        /// <summary>
        /// The descriptor for the status flow of the asset.
        /// </summary>
        public StatusFlowDescriptor StatusFlowDescriptor { get; internal set; }

        /// <summary>
        /// The creation and update information of the asset.
        /// </summary>
        public AuthoringInfo AuthoringInfo { get; internal set; }
    }
}
