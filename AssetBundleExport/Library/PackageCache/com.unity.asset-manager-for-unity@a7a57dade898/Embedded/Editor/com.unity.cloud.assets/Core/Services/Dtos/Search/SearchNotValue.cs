using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Unity.Cloud.AssetsEmbedded
{
    [DataContract]
    class SearchNotValue : ISearchValue
    {
        [DataMember(Name = "condition")]
        public ISearchValue Value { get; set; }

        [DataMember(Name = "type")]
        public string Type => "not";

        public bool IsEmpty() => string.IsNullOrEmpty(Type) || Value == null;
    }
}
