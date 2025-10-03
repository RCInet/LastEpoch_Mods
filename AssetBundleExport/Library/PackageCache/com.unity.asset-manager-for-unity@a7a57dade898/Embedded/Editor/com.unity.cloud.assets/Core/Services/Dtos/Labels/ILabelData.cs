using System.Runtime.Serialization;

namespace Unity.Cloud.AssetsEmbedded
{
    interface ILabelData : ILabelBaseData, IAuthoringData
    {
        [DataMember(Name = "isSystemLabel")]
        bool IsSystemLabel { get; }

        [DataMember(Name = "isUserAssignable")]
        bool IsUserAssignable { get; }
    }
}
