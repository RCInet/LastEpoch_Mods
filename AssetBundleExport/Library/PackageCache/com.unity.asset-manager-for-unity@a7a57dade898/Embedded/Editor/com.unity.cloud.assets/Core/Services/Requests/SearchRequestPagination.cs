using System;
using System.Runtime.Serialization;

namespace Unity.Cloud.AssetsEmbedded
{
    /// <summary>
    /// The pagination of the request.
    /// </summary>
    [DataContract(Name = "search_request_pagination")]
    class SearchRequestPagination
    {
        /// <summary>
        /// The pagination of the request.
        /// </summary>
        /// <param name="sortingField">The field with which to sort the assets.</param>
        /// <param name="sortingOrder">The order in which to sort the assets.</param>
        public SearchRequestPagination(string sortingField, SortingOrder sortingOrder = SortingOrder.Ascending)
        {
            SortingField = sortingField;
            SortingOrderStr = sortingOrder switch
            {
                SortingOrder.Ascending => "Ascending",
                SortingOrder.Descending => "Descending",
                _ => throw new ArgumentOutOfRangeException(nameof(sortingOrder), sortingOrder, null)
            };
        }

        /// <summary>
        /// The pagination token.
        /// </summary>
        [DataMember(Name = "token", EmitDefaultValue = false)]
        public string Token { get; set; }

        /// <summary>
        /// The amount of assets per page.
        /// </summary>
        [DataMember(Name = "limit", EmitDefaultValue = false)]
        public int Limit { get; set; }

        /// <summary>
        /// The field to sort the assets from the page.
        /// </summary>
        [DataMember(Name = "sortingField", IsRequired = true, EmitDefaultValue = true)]
        public string SortingField { get; }

        /// <summary>
        /// The order to sort the assets from the page.
        /// </summary>
        [DataMember(Name = "sortingOrder", IsRequired = true, EmitDefaultValue = true)]
        public string SortingOrderStr { get; }
    }
}
