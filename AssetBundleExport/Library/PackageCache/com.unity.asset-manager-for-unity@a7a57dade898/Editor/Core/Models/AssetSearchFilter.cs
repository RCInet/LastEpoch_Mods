using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;

namespace Unity.AssetManager.Core.Editor
{
    [Serializable]
    class AssetSearchFilter : ISerializationCallbackReceiver
    {
        public List<string> Searches;
        public List<string> AssetIds;
        public List<string> AssetVersions;
        public List<string> CreatedBy;
        public List<string> UpdatedBy;
        public List<string> Status;
        public List<string> Collection;

        public List<string> AssetTypeStrings;
        public List<AssetType> AssetTypes;
        public List<string> Tags;
        public List<string> Labels;

        public bool IsExactMatchSearch;

        [NonSerialized]
        public List<IMetadata> CustomMetadata;

        [SerializeField]
        SerializableSearchFilterMetadata m_SerializableSearchFilterMetadata;

        public void OnBeforeSerialize()
        {
            if (CustomMetadata == null)
                return;

            m_SerializableSearchFilterMetadata = SerializableSearchFilterMetadata.Convert(CustomMetadata);
        }
        public void OnAfterDeserialize()
        {
            if (m_SerializableSearchFilterMetadata == null)
                return;

            CustomMetadata = SerializableSearchFilterMetadata.Convert(m_SerializableSearchFilterMetadata);
        }

        public AssetSearchFilter Clone()
        {
            return new AssetSearchFilter
            {
                Searches = CopyList(Searches),
                AssetIds = CopyList(AssetIds),
                AssetVersions = CopyList(AssetVersions),
                CreatedBy = CopyList(CreatedBy),
                UpdatedBy = CopyList(UpdatedBy),
                Status = CopyList(Status),
                Collection = CopyList(Collection),
                AssetTypeStrings = CopyList(AssetTypeStrings),
                AssetTypes = AssetTypes?.ToList(),
                Tags = CopyList(Tags),
                Labels = CopyList(Labels),
                CustomMetadata = CloneMetadataList(CustomMetadata),
                IsExactMatchSearch = IsExactMatchSearch,
            };
        }

        static List<string> CopyList(List<string> list)
        {
            return list == null ? null : new List<string>(list);
        }

        static List<IMetadata> CloneMetadataList(List<IMetadata> metadataList)
        {
            return metadataList?.Select(metadata => metadata.Clone()).ToList();
        }
    }

    [Serializable]
    class SerializableSearchFilterMetadata
    {
        [SerializeField]
        public List<TrackedStringMetadataPersisted> textMedatatas = new();
        [SerializeField]
        public List<TrackedBooleanMetadataPersisted> booleanMedatatas = new();
        [SerializeField]
        public List<TrackedNumberMetadataPersisted> numberMedatatas = new();
        [SerializeField]
        public List<TrackedUrlMetadataPersisted> urlMedatatas = new() ;
        [SerializeField]
        public List<TrackedStringMetadataPersisted> timestampMedatatas = new();
        [SerializeField]
        public List<TrackedStringMetadataPersisted> userMedatatas = new();
        [SerializeField]
        public List<TrackedStringMetadataPersisted> singleSelectionMedatatas = new();
        [SerializeField]
        public List<TrackedStringListMetadataPersisted> multiSelectionMedatatas = new();

        [Serializable]
        internal class TrackedStringMetadataPersisted
        {
            [SerializeField]
            public string key;

            [SerializeField]
            public string displayName;

            [SerializeField]
            public string value;
        }

        [Serializable]
        internal class TrackedBooleanMetadataPersisted
        {
            [SerializeField]
            public string key;

            [SerializeField]
            public string displayName;

            [SerializeField]
            public bool value;
        }

        [Serializable]
        internal class TrackedNumberMetadataPersisted
        {
            [SerializeField]
            public string key;

            [SerializeField]
            public string displayName;

            [SerializeField]
            public double value;
        }

        [Serializable]
        internal struct UriPersisted
        {
            [SerializeField]
            public string uri;

            [SerializeField]
            public string label;
        }

        [Serializable]
        internal class TrackedUrlMetadataPersisted
        {
            [SerializeField]
            public string key;

            [SerializeField]
            public string displayName;

            [SerializeField]
            public UriPersisted value;
        }

        [Serializable]
        internal class TrackedStringListMetadataPersisted
        {
            [SerializeField]
            public string key;

            [SerializeField]
            public string displayName;

            [SerializeField]
            public List<string> value;
        }

        internal static SerializableSearchFilterMetadata Convert(List<IMetadata> metadatas)
        {
            var trackedMetadata = new SerializableSearchFilterMetadata();

            foreach (var metadata in metadatas)
            {
                switch (metadata)
                {
                    case TextMetadata textMetadata:
                        trackedMetadata.textMedatatas.Add(
                            new TrackedStringMetadataPersisted()
                            {
                                key = textMetadata.FieldKey,
                                displayName = textMetadata.Name,
                                value = textMetadata.Value
                            });
                        break;
                    case BooleanMetadata booleanMetadata:
                        trackedMetadata.booleanMedatatas.Add(
                            new TrackedBooleanMetadataPersisted()
                            {
                                key = booleanMetadata.FieldKey,
                                displayName = booleanMetadata.Name,
                                value = booleanMetadata.Value
                            });
                        break;
                    case NumberMetadata numberMetadata:
                        trackedMetadata.numberMedatatas.Add(
                            new TrackedNumberMetadataPersisted()
                            {
                                key = numberMetadata.FieldKey,
                                displayName = numberMetadata.Name,
                                value = numberMetadata.Value
                            });
                        break;
                    case UrlMetadata urlMetadata:
                        trackedMetadata.urlMedatatas.Add(
                            new TrackedUrlMetadataPersisted()
                            {
                                key = urlMetadata.FieldKey,
                                displayName = urlMetadata.Name,
                                value = new UriPersisted()
                                {
                                    uri = urlMetadata.Value.Uri?.ToString(),
                                    label = urlMetadata.Value.Label
                                }
                            });
                        break;
                    case TimestampMetadata dateTimeMetadata:
                        trackedMetadata.timestampMedatatas.Add(
                            new TrackedStringMetadataPersisted()
                            {
                                key = dateTimeMetadata.FieldKey,
                                displayName = dateTimeMetadata.Name,
                                value = dateTimeMetadata.Value.DateTime.ToString("o")
                            });
                        break;
                    case UserMetadata userMetadata:
                        trackedMetadata.userMedatatas.Add(
                            new TrackedStringMetadataPersisted()
                            {
                                key = userMetadata.FieldKey,
                                displayName = userMetadata.Name,
                                value = userMetadata.Value
                            });
                        break;
                    case SingleSelectionMetadata singleSelectionMetadata:
                        trackedMetadata.singleSelectionMedatatas.Add(
                            new TrackedStringMetadataPersisted()
                            {
                                key = singleSelectionMetadata.FieldKey,
                                displayName = singleSelectionMetadata.Name,
                                value = singleSelectionMetadata.Value
                            });
                        break;
                    case MultiSelectionMetadata multiSelectionMetadata:
                        trackedMetadata.multiSelectionMedatatas.Add(
                            new TrackedStringListMetadataPersisted()
                            {
                                key = multiSelectionMetadata.FieldKey,
                                displayName = multiSelectionMetadata.Name,
                                value = multiSelectionMetadata.Value
                            });
                        break;
                    default:
                        Utilities.DevAssert(false, "Unknown metadata type");
                        return null;
                }
            }
            return trackedMetadata;
        }

        internal static List<IMetadata> Convert(SerializableSearchFilterMetadata trackedMetadataPersisted)
        {
            var metadata = new List<IMetadata>();
            foreach (var textMetadata in trackedMetadataPersisted.textMedatatas)
            {
                metadata.Add(new TextMetadata(textMetadata.key, textMetadata.displayName, textMetadata.value));
            }
            foreach (var booleanMetadata in trackedMetadataPersisted.booleanMedatatas)
            {
                metadata.Add(new BooleanMetadata(booleanMetadata.key, booleanMetadata.displayName, booleanMetadata.value));
            }
            foreach (var numberMetadata in trackedMetadataPersisted.numberMedatatas)
            {
                metadata.Add(new NumberMetadata(numberMetadata.key, numberMetadata.displayName, numberMetadata.value));
            }
            foreach (var urlMetadata in trackedMetadataPersisted.urlMedatatas)
            {
                metadata.Add(new UrlMetadata(urlMetadata.key, urlMetadata.displayName,
                    new UriEntry(new Uri(urlMetadata.value.uri), urlMetadata.value.label)));
            }
            foreach (var dateTimeMetadata in trackedMetadataPersisted.timestampMedatatas)
            {
                metadata.Add(
                    new TimestampMetadata(
                        dateTimeMetadata.key,
                        dateTimeMetadata.displayName,
                        new DateTimeEntry(DateTime.Parse(dateTimeMetadata.value, DateTimeFormatInfo.CurrentInfo, DateTimeStyles.RoundtripKind))));
            }
            foreach (var userMetadata in trackedMetadataPersisted.userMedatatas)
            {
                metadata.Add(new UserMetadata(userMetadata.key, userMetadata.displayName, userMetadata.value));
            }
            foreach (var singleSelectionMetadata in trackedMetadataPersisted.singleSelectionMedatatas)
            {
                metadata.Add(new SingleSelectionMetadata(singleSelectionMetadata.key, singleSelectionMetadata.displayName, singleSelectionMetadata.value));
            }
            foreach (var multiSelectionMetadata in trackedMetadataPersisted.multiSelectionMedatatas)
            {
                metadata.Add(new MultiSelectionMetadata(multiSelectionMetadata.key, multiSelectionMetadata.displayName, multiSelectionMetadata.value));
            }
            return metadata;
        }
    }
}
