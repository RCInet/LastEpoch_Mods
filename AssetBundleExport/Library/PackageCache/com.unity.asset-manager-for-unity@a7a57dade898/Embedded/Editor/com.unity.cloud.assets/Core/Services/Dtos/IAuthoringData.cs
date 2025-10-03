using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AssetsEmbedded
{
    interface IAuthoringData
    {
        [DataMember(Name = "createdBy")]
        string CreatedBy => null;

        [DataMember(Name = "created")]
        DateTime? Created => null;

        [DataMember(Name = "updatedBy")]
        string UpdatedBy => null;

        [DataMember(Name = "updated")]
        DateTime? Updated => null;
    }
}
