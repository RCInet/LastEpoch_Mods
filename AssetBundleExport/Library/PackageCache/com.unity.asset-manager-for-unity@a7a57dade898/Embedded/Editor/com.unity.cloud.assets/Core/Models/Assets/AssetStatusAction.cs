using System;
using System.Runtime.Serialization;

namespace Unity.Cloud.AssetsEmbedded
{
    [Obsolete("Use IStatus instead.")]
    [DataContract]
enum AssetStatusAction
    {
        [EnumMember(Value = "inreview")]
        SendForReview,
        [EnumMember(Value = "approved")]
        Approve,
        [EnumMember(Value = "rejected")]
        Reject,
        [EnumMember(Value = "published")]
        Publish,
        [EnumMember(Value = "withdrawn")]
        Withdraw
    }
}
