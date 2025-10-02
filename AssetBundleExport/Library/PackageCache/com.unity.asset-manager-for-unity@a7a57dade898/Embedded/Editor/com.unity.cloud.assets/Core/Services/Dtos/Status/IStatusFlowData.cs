using System.Runtime.Serialization;

namespace Unity.Cloud.AssetsEmbedded
{
    interface IStatusFlowData
    {
        [DataMember(Name = "id")]
        string Id { get; }

        [DataMember(Name = "name")]
        string Name { get; }

        [DataMember(Name = "startAssetStatusId")]
        string StartStatusId { get; }

        [DataMember(Name = "isDefault")]
        bool IsDefault { get; }

        IStatusData[] Statuses { get; }
        IStatusTransitionData[] Transitions { get; }
    }
}
