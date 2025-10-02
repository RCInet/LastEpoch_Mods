using System;

namespace Unity.Cloud.AssetsEmbedded
{
    /// <summary>
    /// A structure for defining the criteria of an <see cref="IFile"/> search request.
    /// </summary>
    class AuthoringInfoSearchFilter : CompoundSearchCriteria
    {
        /// <inheritdoc cref="AuthoringInfo.Created"/>
        public ConditionalSearchCriteria<DateTime> Created { get; } = new(nameof(AuthoringInfo.Created), "created", SearchConditionData.DateRangeType);

        /// <inheritdoc cref="AuthoringInfo.CreatedBy"/>
        public SearchCriteria<string> CreatedBy { get; } = new(nameof(AuthoringInfo.CreatedBy), "createdBy");

        /// <inheritdoc cref="AuthoringInfo.Updated"/>
        public ConditionalSearchCriteria<DateTime> Updated { get; } = new(nameof(AuthoringInfo.Updated), "updated", SearchConditionData.DateRangeType);

        /// <inheritdoc cref="AuthoringInfo.UpdatedBy"/>
        public SearchCriteria<string> UpdatedBy { get; } = new(nameof(AuthoringInfo.UpdatedBy), "updatedBy");

        internal AuthoringInfoSearchFilter(string propertyName, string searchKey)
            : base(propertyName, searchKey) { }
    }
}
