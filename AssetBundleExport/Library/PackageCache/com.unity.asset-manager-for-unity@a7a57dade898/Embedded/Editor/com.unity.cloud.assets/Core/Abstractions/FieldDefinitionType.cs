using System.Runtime.Serialization;

namespace Unity.Cloud.AssetsEmbedded
{
    [DataContract]
enum FieldDefinitionType
    {
        [EnumMember(Value = "boolean")]
        Boolean,
        [EnumMember(Value = "selection")]
        Selection,
        [EnumMember(Value = "number")]
        Number,
        [EnumMember(Value = "text")]
        Text,
        [EnumMember(Value = "timestamp")]
        Timestamp,
        [EnumMember(Value = "url")]
        Url,
        [EnumMember(Value = "user")]
        User
    }
}
