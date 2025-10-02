using System;
using System.Runtime.Serialization;

namespace Unity.Cloud.AssetsEmbedded
{
    // Dev note: [DataContract] and [EnumMember] are artifacts of the old serialization strategy.
    // The attributes are maintained for compatibility reasons and to avoid a breaking change.
    [DataContract]
enum WorkflowType
    {
        /// <summary>
        /// Produce four 2d images from the X, Y, Z, and Isometric-front-left directions for previewing purposes.
        /// </summary>
        [EnumMember(Value = "thumbnail-generator")]
        Thumbnail_Generation,
        /// <summary>
        /// Produce a simplified GLB representation of a 3D model.
        /// </summary>
        [EnumMember(Value = "glb-preview")]
        GLB_Preview,
        /// <summary>
        /// Produce a representation of a large 3D model utilizing hierarchical levels of detail for dynamic streaming.
        /// Only available in the Unity Enterprise Tier.
        /// </summary>
        [EnumMember(Value = "3d-data-streaming")]
        Data_Streaming,
        /// <summary>
        /// Transcode videos so that they can be scrubbed at high fidelity in the web viewer.
        /// </summary>
        [EnumMember(Value = "transcode-video")]
        Transcode_Video,
        /// <summary>
        /// Generate a metadata JSON file containing extracted 3D model metadata. The metadata is also added to the asset.
        /// </summary>
        Metadata_Extraction,
        [Obsolete("Use Optimize_Convert_Free or Optimize_Convert_Pro instead.")]
        Generic_Polygon_Target,
        Custom,
        /// <summary>
        /// Optimize and convert generic file formats.
        /// </summary>
        Optimize_Convert_Free,
        /// <summary>
        /// Optimize and convert all supported 3D file formats.
        /// Only available in the Unity Pro and Enterprise/Industry Tiers.
        /// </summary>
        Optimize_Convert_Pro,
    }
}
