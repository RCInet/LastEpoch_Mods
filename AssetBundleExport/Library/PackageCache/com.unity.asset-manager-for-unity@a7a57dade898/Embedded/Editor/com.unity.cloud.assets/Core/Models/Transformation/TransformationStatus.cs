using System.Runtime.Serialization;

namespace Unity.Cloud.AssetsEmbedded
{
    // Dev note: [DataContract] and [EnumMember] are artifacts of the old serialization strategy.
    // The attributes are maintained for compatibility reasons and to avoid a breaking change.
    [DataContract]
enum TransformationStatus
    {
        [EnumMember(Value = "pending")]
        Pending,
        [EnumMember(Value = "running")]
        Running,
        [EnumMember(Value = "succeeded")]
        Succeeded,
        [EnumMember(Value = "failed")]
        Failed,
        [EnumMember(Value = "error")]
        Error,
        [EnumMember(Value = "terminated")]
        Terminated,
        [EnumMember(Value = "skipped")]
        Skipped,
        [EnumMember(Value = "timedout")]
        TimedOut,
        [EnumMember(Value = "terminating")]
        Terminating,
        Queued
    }
}
