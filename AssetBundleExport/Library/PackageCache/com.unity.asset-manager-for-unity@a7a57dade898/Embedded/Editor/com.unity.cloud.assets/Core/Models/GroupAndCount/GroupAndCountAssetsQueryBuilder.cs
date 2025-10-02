using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.AssetsEmbedded
{
    /// <summary>
    /// A class that builds and executes a query to return an asset count.
    /// </summary>
    class GroupAndCountAssetsQueryBuilder
    {
        readonly IAssetDataSource m_AssetDataSource;
        readonly OrganizationId m_OrganizationId;
        readonly List<ProjectId> m_ProjectIds = new();

        IAssetSearchFilter m_AssetSearchFilter;
        int? m_Limit;

        GroupAndCountAssetsQueryBuilder(IAssetDataSource assetDataSource)
        {
            m_AssetDataSource = assetDataSource;
        }

        internal GroupAndCountAssetsQueryBuilder(IAssetDataSource assetDataSource, ProjectDescriptor projectDescriptor)
            : this(assetDataSource)
        {
            m_OrganizationId = projectDescriptor.OrganizationId;
            m_ProjectIds.Add(projectDescriptor.ProjectId);
        }

        internal GroupAndCountAssetsQueryBuilder(IAssetDataSource assetDataSource, IEnumerable<ProjectDescriptor> projectDescriptors)
            : this(assetDataSource)
        {
            var projects = projectDescriptors.ToArray();
            if (projects.Length == 0)
            {
                throw new ArgumentNullException(nameof(projectDescriptors), "No project descriptors were provided.");
            }

            m_OrganizationId = projects[0].OrganizationId;
            for (var i = 1; i < projects.Length; i++)
            {
                if (projects[i].OrganizationId != m_OrganizationId)
                {
                    throw new InvalidOperationException("The projects do not belong to the same organization.");
                }
            }

            m_ProjectIds.AddRange(projects.Select(descriptor => descriptor.ProjectId));
        }

        internal GroupAndCountAssetsQueryBuilder(IAssetDataSource assetDataSource, OrganizationId organizationId)
            : this(assetDataSource)
        {
            m_OrganizationId = organizationId;
        }

        /// <summary>
        /// Sets the filter to be used when querying assets.
        /// </summary>
        /// <param name="assetSearchFilter">The query filter. </param>
        /// <returns>The calling <see cref="GroupAndCountAssetsQueryBuilder"/>. </returns>
        public GroupAndCountAssetsQueryBuilder SelectWhereMatchesFilter(IAssetSearchFilter assetSearchFilter)
        {
            m_AssetSearchFilter = assetSearchFilter;
            return this;
        }

        /// <summary>
        /// Sets the limit of the counters.
        /// </summary>
        /// <param name="limit">The max count to return for each value. </param>
        /// <returns>The calling <see cref="GroupAndCountAssetsQueryBuilder"/>. </returns>
        public GroupAndCountAssetsQueryBuilder LimitTo(int limit)
        {
            m_Limit = limit;
            return this;
        }

        /// <summary>
        /// Executes the query and returns the result.
        /// </summary>
        /// <param name="groupBy">A <see cref="GroupableField"/> by which to group the assets. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request. </param>
        /// <returns>An async enumeration of <see cref="GroupableFieldValue"/> and its corresponding asset count. </returns>
        public IAsyncEnumerable<KeyValuePair<GroupableFieldValue, int>> ExecuteAsync(Groupable groupBy, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(groupBy.Value))
            {
                throw new ArgumentNullException(nameof(groupBy), "No group by field was provided.");
            }

            return ExecuteInternalAsync(groupBy.Value, cancellationToken);
        }

        /// <summary>
        /// Executes the query and returns the result.
        /// </summary>
        /// <param name="groupBy">A combination of <see cref="GroupableField"/> and metadata field keys by which to group the assets. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request. </param>
        /// <returns>An async enumeration of <see cref="GroupableFieldValue"/> and its corresponding asset count. </returns>
        public IAsyncEnumerable<KeyValuePair<IEnumerable<GroupableFieldValue>, int>> ExecuteAsync(IEnumerable<Groupable> groupBy, CancellationToken cancellationToken)
        {
            var groupByArray = groupBy?.Where(x => !string.IsNullOrEmpty(x.Value)).ToArray() ?? Array.Empty<Groupable>();
            if (groupByArray.Length == 0)
            {
                throw new ArgumentNullException(nameof(groupBy), "No group by fields were provided.");
            }

            return ExecuteInternalAsync(groupByArray.Select(x => x.Value).ToArray(), cancellationToken);
        }

        /// <summary>
        /// Executes the query and returns the result.
        /// </summary>
        /// <param name="groupBy">The field by which to group the assets. </param>
        /// <param name="cancellationToken">A token that can be used to cancel the request. </param>
        /// <returns>A task whose result is a dictionary of groups and their counts. </returns>
        [Obsolete("Use ExecuteAsync((Groupable)groupBy, cancellationToken) instead.")]
        public async Task<IReadOnlyDictionary<string, int>> ExecuteAsync(GroupableField groupBy, CancellationToken cancellationToken)
        {
            var data = new Dictionary<string, int>();
            await foreach (var kvp in ExecuteAsync((Groupable) groupBy, cancellationToken))
            {
                data.TryAdd(kvp.Key.AsString(), kvp.Value);
            }

            return data;
        }

        /// <summary>
        /// Executes the query and returns the result.
        /// </summary>
        /// <param name="cancellationToken">A token that can be used to cancel the request. </param>
        /// <returns>A task whose result is a dictionary of . </returns>
        [Obsolete("Use ExecuteAsync(GroupableField.Collections, cancellationToken) instead.")]
        public async Task<IReadOnlyDictionary<CollectionDescriptor, int>> GroupByCollectionAndExecuteAsync(CancellationToken cancellationToken)
        {
            var data = new Dictionary<CollectionDescriptor, int>();
            await foreach (var kvp in ExecuteAsync((Groupable) GroupableField.Collections, cancellationToken))
            {
                data.TryAdd(kvp.Key.AsCollectionDescriptor(), kvp.Value);
            }

            return data;
        }

        async IAsyncEnumerable<KeyValuePair<GroupableFieldValue, int>> ExecuteInternalAsync(string groupBy, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var metadataFieldTypes = new Dictionary<string, MetadataValueType>();

            var aggregations = await GetAssetAggregateAsync(new[] {groupBy}, cancellationToken);

            for (var i = 0; i < aggregations.Length; ++i)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var groupValue = await ParseGroupValueAsync(groupBy, aggregations[i].Value, metadataFieldTypes, cancellationToken);

                yield return new KeyValuePair<GroupableFieldValue, int>(groupValue, aggregations[i].Count);
            }
        }

        async IAsyncEnumerable<KeyValuePair<IEnumerable<GroupableFieldValue>, int>> ExecuteInternalAsync(string[] groupBy, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var metadataFieldTypes = new Dictionary<string, MetadataValueType>();

            var aggregations = await GetAssetAggregateAsync(groupBy, cancellationToken);

            for (var i = 0; i < aggregations.Length; ++i)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var groupValues = new List<GroupableFieldValue>();

                var value = IsolatedSerialization.ToObjectDictionary(aggregations[i].Value) ?? new Dictionary<string, object>();
                foreach (var kvp in value)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var groupValue = await ParseGroupValueAsync(kvp.Key, kvp.Value, metadataFieldTypes, cancellationToken);
                    groupValues.Add(groupValue);
                }

                yield return new KeyValuePair<IEnumerable<GroupableFieldValue>, int>(groupValues, aggregations[i].Count);
            }
        }

        async Task<AggregateDto[]> GetAssetAggregateAsync(string[] aggregationFieldStrings, CancellationToken cancellationToken)
        {
            object aggregateBy = aggregationFieldStrings.Select(s => s.Trim()).ToArray();
            if (aggregationFieldStrings.Length == 1)
            {
                aggregateBy = aggregationFieldStrings[0];
            }

            var projectIds = m_ProjectIds.ToArray();

            switch (projectIds.Length)
            {
                case 1:
                {
                    var parameters = new SearchAndAggregateRequestParameters(aggregateBy)
                    {
                        Filter = m_AssetSearchFilter?.From(),
                        MaximumNumberOfItems = m_Limit,
                    };
                    var descriptor = new ProjectDescriptor(m_OrganizationId, projectIds[0]);
                    return await m_AssetDataSource.GetAssetAggregateAsync(descriptor, parameters, cancellationToken);
                }
                default:
                {
                    var parameters = new AcrossProjectsSearchAndAggregateRequestParameters(projectIds, aggregateBy)
                    {
                        Filter = m_AssetSearchFilter?.From(),
                        MaximumNumberOfItems = m_Limit,
                    };
                    return await m_AssetDataSource.GetAssetAggregateAsync(m_OrganizationId, parameters, cancellationToken);
                }
            }
        }

        async Task<GroupableFieldValue> ParseGroupValueAsync(string key, object value, Dictionary<string, MetadataValueType> metadataFieldTypes, CancellationToken cancellationToken)
        {
            // Try to parse a metadata field key
            string metadataFieldKey = null;
            if (key.Contains("metadata."))
            {
                metadataFieldKey = key.Split("metadata.")[^1];
            }
            else if (key.Contains("systemMetadata."))
            {
                metadataFieldKey = key.Split("systemMetadata.")[^1];
            }

            if (!string.IsNullOrEmpty(metadataFieldKey))
            {
                if (!metadataFieldTypes.TryGetValue(metadataFieldKey, out var metadataValueType))
                {
                    metadataValueType = await m_AssetDataSource.GetMetadataValueTypeAsync(new FieldDefinitionDescriptor(m_OrganizationId, metadataFieldKey), cancellationToken);
                    metadataFieldTypes.Add(metadataFieldKey, metadataValueType);
                }

                var metadataValue = new MetadataObject(metadataValueType, value.ToString());
                return new GroupableFieldValue(GroupableFieldValueType.MetadataValue, metadataValue);
            }

            // Else
            return ParseGroupValue(key, value);
        }

        GroupableFieldValue ParseGroupValue(string key, object value)
        {
            if (key.EndsWith("createdBy") || key.EndsWith("updatedBy"))
            {
                return new GroupableFieldValue(GroupableFieldValueType.UserId, new UserId(AsString(value)));
            }

            return key switch
            {
                "assetId" => new GroupableFieldValue(GroupableFieldValueType.AssetId, new AssetId(AsString(value))),
                "assetVersion" => new GroupableFieldValue(GroupableFieldValueType.AssetVersion, new AssetVersion(AsString(value))),
                "datasetId" => new GroupableFieldValue(GroupableFieldValueType.DatasetId, new DatasetId(AsString(value))),
                "collections" => new GroupableFieldValue(GroupableFieldValueType.CollectionDescriptor, AsCollectionDescriptor(value)),
                "primaryType" => value.ToString().TryGetAssetTypeFromString(out var assetType)
                    ? new GroupableFieldValue(GroupableFieldValueType.AssetType, assetType)
                    : new GroupableFieldValue(GroupableFieldValueType.String, AsString(value)),
                "datasets.primaryType" => value.ToString().TryGetAssetTypeFromString(out var assetType)
                    ? new GroupableFieldValue(GroupableFieldValueType.AssetType, assetType)
                    : new GroupableFieldValue(GroupableFieldValueType.String, AsString(value)),
                _ => new GroupableFieldValue(GroupableFieldValueType.String, AsString(value))
            };
        }

        CollectionDescriptor AsCollectionDescriptor(object value)
        {
            var split = AsString(value).Split('/');
            if (split.Length >= 2)
            {
                var projectIdStr = split[0];
                projectIdStr = projectIdStr.Replace("proj-", "");

                var collectionPath = CollectionPath.BuildPath(split.Skip(1).ToArray());
                return new CollectionDescriptor(new ProjectDescriptor(m_OrganizationId, new ProjectId(projectIdStr)), collectionPath);
            }

            throw new ArgumentException("The key is not a collection descriptor.");
        }

        static string AsString(object value)
        {
            return Uri.UnescapeDataString(value?.ToString() ?? "");
        }
    }
}
