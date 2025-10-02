using System;

namespace Unity.Cloud.AssetsEmbedded
{
    struct PaginationData
    {
        public string SortingField { get; set; }
        public SortingOrder SortingOrder { get; set; }
        public Range Range { get; set; }
    }
}
