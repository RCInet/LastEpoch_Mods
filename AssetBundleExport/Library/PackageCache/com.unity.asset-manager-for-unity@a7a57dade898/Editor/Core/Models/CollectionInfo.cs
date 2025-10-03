using System;

namespace Unity.AssetManager.Core.Editor
{
    [Serializable]
    class CollectionInfo
    {
        public string OrganizationId;
        public string ProjectId;
        public string Name;
        public string ParentPath;

        public string GetFullPath()
        {
            return string.IsNullOrEmpty(ParentPath) ? Name ?? string.Empty : $"{ParentPath}/{Name ?? string.Empty}";
        }

        public string GetUniqueIdentifier()
        {
            if (string.IsNullOrEmpty(OrganizationId) || string.IsNullOrEmpty(ProjectId))
            {
                return string.Empty;
            }

            return $"{OrganizationId}/{ProjectId}/{GetFullPath()}";
        }

        public static bool AreEquivalent(CollectionInfo left, CollectionInfo right)
        {
            var leftIdentifier = left?.GetUniqueIdentifier() ?? string.Empty;
            var rightIdentifier = right?.GetUniqueIdentifier() ?? string.Empty;
            return leftIdentifier == rightIdentifier;
        }

        public static CollectionInfo CreateFromFullPath(string fullPath)
        {
            if (!string.IsNullOrEmpty(fullPath))
            {
                var slashIndex = fullPath.LastIndexOf('/');
                if (slashIndex > 0)
                {
                    var parentPath = fullPath.Substring(0, slashIndex);
                    var name = fullPath.Substring(slashIndex + 1);
                    return new CollectionInfo { Name = name, ParentPath = parentPath };
                }
            }

            return new CollectionInfo { Name = fullPath, ParentPath = string.Empty };
        }
    }
}
