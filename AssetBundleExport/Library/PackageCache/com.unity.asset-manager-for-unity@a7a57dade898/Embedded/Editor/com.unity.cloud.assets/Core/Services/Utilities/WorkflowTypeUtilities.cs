using System;

namespace Unity.Cloud.AssetsEmbedded
{
    static class WorkflowTypeUtilities
    {
        public static string ToJsonValue(this WorkflowType workflowType)
        {
            return workflowType switch
            {
                WorkflowType.Thumbnail_Generation => "thumbnail-generator",
                WorkflowType.GLB_Preview => "glb-preview",
                WorkflowType.Data_Streaming => "3d-data-streaming",
                WorkflowType.Transcode_Video => "video-transcoding",
                WorkflowType.Metadata_Extraction => "metadata-extraction",
                WorkflowType.Optimize_Convert_Free => "free-tier-optimize-and-convert",
                WorkflowType.Optimize_Convert_Pro => "higher-tier-optimize-and-convert",
#pragma warning disable CS0618 // Type or member is obsolete - maintained for backwards compatibility
                WorkflowType.Generic_Polygon_Target => "generic-polygon-target",
#pragma warning restore CS0618 // Type or member is obsolete
                _ => string.Empty
            };
        }

        public static WorkflowType FromJsonValue(string value)
        {
            return value switch
            {
                "thumbnail-generator" => WorkflowType.Thumbnail_Generation,
                "glb-preview" => WorkflowType.GLB_Preview,
                "3d-data-streaming" => WorkflowType.Data_Streaming,
                "video-transcoding" => WorkflowType.Transcode_Video,
                "metadata-extraction" => WorkflowType.Metadata_Extraction,
                "free-tier-optimize-and-convert" => WorkflowType.Optimize_Convert_Free,
                "higher-tier-optimize-and-convert" => WorkflowType.Optimize_Convert_Pro,
#pragma warning disable CS0618 // Type or member is obsolete - maintained for backwards compatibility
                "generic-polygon-target" => WorkflowType.Generic_Polygon_Target,
#pragma warning restore CS0618 // Type or member is obsolete
                _ => default
            };
        }
    }
}
