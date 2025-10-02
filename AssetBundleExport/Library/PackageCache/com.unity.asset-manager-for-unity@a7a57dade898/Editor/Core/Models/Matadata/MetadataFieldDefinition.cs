using System;
using System.Collections.Generic;
using System.Linq;

namespace Unity.AssetManager.Core.Editor
{
    [Serializable]
    enum MetadataFieldType
    {
        Text,
        Boolean,
        Number,
        Timestamp,
        Url,
        User,
        SingleSelection,
        MultiSelection
    }

    interface IMetadataFieldDefinition
    {
        public string Key { get; }

        public string DisplayName { get; }

        public MetadataFieldType Type { get; }
    }

    [Serializable]
    class MetadataFieldDefinition : IMetadataFieldDefinition
    {
        string m_Key;
        string m_DisplayName;
        MetadataFieldType m_Type;

        public string Key => m_Key;
        public string DisplayName => m_DisplayName;
        public MetadataFieldType Type => m_Type;

        public MetadataFieldDefinition(string key, string displayName, MetadataFieldType fieldType)
        {
            m_Key = key;
            m_DisplayName = displayName;
            m_Type = fieldType;
        }
    }

    [Serializable]
    class SelectionFieldDefinition : IMetadataFieldDefinition
    {
        string m_Key;
        string m_DisplayName;
        MetadataFieldType m_Type;
        List<string> m_AcceptedValues;

        public string Key => m_Key;
        public string DisplayName => m_DisplayName;
        public MetadataFieldType Type  => m_Type;
        public IEnumerable<string> AcceptedValues => m_AcceptedValues ?? new List<string>();

        public SelectionFieldDefinition(string key, string displayName, MetadataFieldType fieldType,
            IEnumerable<string> acceptedValues)
        {
            m_Key = key;
            m_DisplayName = displayName;
            m_Type = fieldType;
            m_AcceptedValues = acceptedValues?.ToList();
        }
    }
}
