using System;
using Unity.AssetManager.Core.Editor;

namespace Unity.AssetManager.UI.Editor
{
    static class ProjectIcons
    {
        internal const string k_ConnectedIcon = "Images/Icons/ProjectWindowOverlay/ConnectedIcon@2x.png";
        internal const string k_ImportedIcon = "Images/Icons/ProjectWindowOverlay/ImportedIcon@2x.png";
        internal const string k_OutOfDateIcon = "Images/Icons/ProjectWindowOverlay/OutOfDateIcon@2x.png";
        internal const string k_ErrorIcon = "Images/Icons/ProjectWindowOverlay/ErrorIcon@2x.png";
    }

    struct ProjectIconAssetStatus
    {
        public ImportAttribute.ImportStatus Status { get; }
        public string Description { get; }
        public string IconPath { get; }

        public ProjectIconAssetStatus(ImportAttribute.ImportStatus status, string description, string iconPath)
        {
            Status = status;
            Description = description;
            IconPath = iconPath;
        }
    }
}
