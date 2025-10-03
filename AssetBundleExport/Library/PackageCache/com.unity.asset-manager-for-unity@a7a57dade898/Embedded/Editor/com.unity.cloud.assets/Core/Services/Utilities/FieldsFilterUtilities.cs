using System;
using System.Collections.Generic;

namespace Unity.Cloud.AssetsEmbedded
{
    static class FieldsFilterUtilities
    {
        public delegate void OnFieldFilterSelected(string field);

        internal static void Parse(this FieldsFilter fieldsFilter, OnFieldFilterSelected select)
        {
            if (fieldsFilter == null) return;

            fieldsFilter.AssetFields.Parse(select);
            fieldsFilter.DatasetFields.Parse(select, "datasets.");
            fieldsFilter.FileFields.Parse(select, "files.");

            fieldsFilter.MetadataFields.Select(select, "metadata",
                fieldsFilter.AssetFields.HasFlag(AssetFields.metadata),
                fieldsFilter.DatasetFields.HasFlag(DatasetFields.metadata),
                fieldsFilter.FileFields.HasFlag(FileFields.metadata));

            fieldsFilter.SystemMetadataFields.Select(select, "systemMetadata",
                fieldsFilter.AssetFields.HasFlag(AssetFields.systemMetadata),
                fieldsFilter.DatasetFields.HasFlag(DatasetFields.systemMetadata),
                fieldsFilter.FileFields.HasFlag(FileFields.systemMetadata));
        }

        internal static void Parse(this FileFields fileFields, OnFieldFilterSelected select, string prefix = "")
        {
            if (fileFields.HasFlag(FileFields.all))
            {
                select($"{prefix}*");
                // Explicitly include these as they fail to be returned when using the wildcard.
                select($"{prefix}downloadURL");
                select($"{prefix}previewURL");
                return;
            }

            foreach (FileFields value in Enum.GetValues(typeof(FileFields)))
            {
                if (value is FileFields.all or FileFields.none) continue;
                if (fileFields.HasFlag(value))
                {
                    if (value == FileFields.authoring)
                    {
                        IncludeAuthoringFields(prefix, select);
                    }
                    else
                    {
                        select($"{prefix}{value.ToString()}");
                    }
                }
            }
        }

        internal static void Parse(this DatasetFields datasetFields, OnFieldFilterSelected select, string prefix = "")
        {
            if (datasetFields.HasFlag(DatasetFields.all))
            {
                select($"{prefix}*");
                return;
            }

            foreach (DatasetFields value in Enum.GetValues(typeof(DatasetFields)))
            {
                if (value is DatasetFields.all or DatasetFields.none) continue;
                if (datasetFields.HasFlag(value))
                {
                    if (value == DatasetFields.authoring)
                    {
                        IncludeAuthoringFields(prefix, select);
                    }
                    else
                    {
                        select($"{prefix}{value.ToString()}");
                    }
                }
            }
        }

        static void Parse(this AssetFields assetFields, OnFieldFilterSelected select)
        {
            if (assetFields.HasFlag(AssetFields.all))
            {
                select("*");
                return;
            }

            foreach (AssetFields value in Enum.GetValues(typeof(AssetFields)))
            {
                if (value is AssetFields.all or AssetFields.none) continue;
                if (assetFields.HasFlag(value))
                {
                    switch (value)
                    {
                        case AssetFields.versioning:
                            select("isFrozen");
                            select("autoSubmit");
                            select("versionNumber");
                            select("changeLog");
                            select("parentAssetVersion");
                            select("parentVersionNumber");
                            break;
                        case AssetFields.labels:
                            select("labels");
                            select("archivedLabels");
                            break;
                        case AssetFields.previewFile:
                            select("previewFileDatasetId");
                            select("previewFile");
                            break;
                        case AssetFields.authoring:
                            IncludeAuthoringFields("", select);
                            break;
                        default:
                            select(value.ToString());
                            break;
                    }
                }
            }
        }

        static void IncludeAuthoringFields(string prefix, OnFieldFilterSelected action)
        {
            action(prefix + "created");
            action(prefix + "createdBy");
            action(prefix + "updated");
            action(prefix + "updatedBy");
        }

        static void Select(this IEnumerable<string> metadataKeys, OnFieldFilterSelected select, string metadataprefix, bool hasAssetFlag, bool hasDatasetFlag, bool hasFileFlag)
        {
            foreach (var field in metadataKeys)
            {
                if (hasAssetFlag) select($"{metadataprefix}.{field}");
                if (hasDatasetFlag) select($"datasets.{metadataprefix}.{field}");
                if (hasFileFlag) select($"files.{metadataprefix}.{field}");
            }
        }
    }
}
