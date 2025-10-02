using System;
using System.Collections.Generic;
using System.Linq;

namespace Unity.AssetManager.Core.Editor
{
    enum MetadataSharingType
    {
        None,
        Partial,
        All
    }

    static class MetadataHelpers
    {
        public static MetadataSharingType GetMetadataSharingType(IReadOnlyCollection<IMetadataContainer> metadata, string fieldKey)
        {
            if (MetadataIsInAllAssets(metadata, fieldKey))
            {
                return MetadataSharingType.All;
            }

            return MetadataIsInAtLeastOneAsset(metadata, fieldKey)
                ? MetadataSharingType.Partial
                : MetadataSharingType.None;
        }

        public static bool HasSameMetadataFieldKeys(IReadOnlyCollection<IMetadataContainer> metadata)
        {
            if (metadata.Count == 1)
                return true;

            // We only need to compare the metadata to the first one
            var referenceMetadata = metadata.First();
            var count = referenceMetadata.Count();

            if (metadata.Any(m => m.Count() != count))
                return false;

            foreach (var fieldKey in referenceMetadata.Select(x => x.FieldKey))
            {
                if (!MetadataIsInAllAssets(metadata, fieldKey))
                    return false;
            }

            return true;
        }

        static bool MetadataIsInAllAssets(IEnumerable<IMetadataContainer> metadata, string fieldKey)
        {
            if (metadata == null)
                return false;

            return metadata.All(m => m?.FirstOrDefault(x => x.FieldKey == fieldKey) != null);
        }

        static bool MetadataIsInAtLeastOneAsset(IEnumerable<IMetadataContainer> metadata, string fieldKey)
        {
            if (metadata == null)
                return false;

            return metadata.Any(m => m?.FirstOrDefault(x => x.FieldKey == fieldKey) != null);
        }
    }
}
