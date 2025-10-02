using System;
using System.Linq;
using System.Runtime.Serialization;

namespace Unity.Cloud.AssetsEmbedded
{
    [DataContract]
    class StatusFlowData : IStatusFlowData
    {
        [DataMember(Name = "assetStatuses")]
        public StatusData[] StatusDatas { get; set; }

        [DataMember(Name = "transitions")]
        public StatusTransitionData[] TransitionDatas { get; set; }

        public string Id { get; set; }
        public string Name { get; set; }
        public string StartStatusId { get; set; }
        public bool IsDefault { get; set; }
        public IStatusData[] Statuses => GetStatuses();
        public IStatusTransitionData[] Transitions => GetTransitions();

        IStatusData[] GetStatuses() => StatusDatas?.Select(x => (IStatusData) x).ToArray() ?? Array.Empty<IStatusData>();
        IStatusTransitionData[] GetTransitions() => TransitionDatas?.Select(x => (IStatusTransitionData) x).ToArray() ?? Array.Empty<IStatusTransitionData>();
    }
}
