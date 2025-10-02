using System;
using System.Linq;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AssetsEmbedded
{
    static partial class EntityMapper
    {
        static void MapFrom(this StatusFlow statusFlow, IStatusFlowData data)
        {
            statusFlow.Properties = data.From();
            statusFlow.Statuses = data.Statuses?.Select(s => s.From(statusFlow.Descriptor)).ToArray() ?? Array.Empty<IStatus>();
            statusFlow.Transitions = data.Transitions?.Select(t => t.From(statusFlow.Descriptor)).ToArray() ?? Array.Empty<IStatusTransition>();
        }

        static void MapFrom(this Status status, IStatusData data)
        {
            status.Name = data.Name;
            status.Description = data.Description;
            status.CanBeSkipped = data.CanBeSkipped;
            status.SortingOrder = data.SortingOrder;
            status.InPredicate = new StatusPredicate(data.InPredicate.Id, data.InPredicate.Name);
            status.OutPredicate = new StatusPredicate(data.OutPredicate.Id, data.OutPredicate.Name);
        }

        static void MapFrom(this StatusTransition transition, IStatusTransitionData data)
        {
            transition.FromStatusId = data.FromStatusId;
            transition.ToStatusId = data.ToStatusId;
            transition.ThroughPredicate = new StatusPredicate(data.ThroughPredicate.Id, data.ThroughPredicate.Name);
        }

        internal static StatusFlowProperties From(this IStatusFlowData data)
        {
            return new StatusFlowProperties
            {
                Name = data.Name,
                IsDefault = data.IsDefault,
                StartStatusId = data.StartStatusId
            };
        }

        internal static IStatusFlow From(this IStatusFlowData data, OrganizationId organizationId)
        {
            var descriptor = new StatusFlowDescriptor(organizationId, data.Id);
            var statusFlow = new StatusFlow(descriptor);
            statusFlow.MapFrom(data);
            return statusFlow;
        }

        static IStatus From(this IStatusData data, StatusFlowDescriptor statusFlowDescriptor)
        {
            var status = new Status(statusFlowDescriptor, data.Id);
            status.MapFrom(data);
            return status;
        }

        static IStatusTransition From(this IStatusTransitionData data, StatusFlowDescriptor statusFlowDescriptor)
        {
            var transition = new StatusTransition(statusFlowDescriptor, data.Id);
            transition.MapFrom(data);
            return transition;
        }
    }
}
